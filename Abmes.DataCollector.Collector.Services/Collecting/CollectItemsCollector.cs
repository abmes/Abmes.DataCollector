using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Destinations;
using Abmes.DataCollector.Common.Data.Storage;
using Abmes.DataCollector.Utils;
using System.Collections.Concurrent;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public class CollectItemsCollector(
    IFileNameProvider fileNameProvider,
    IAsyncExecutionStrategy<CollectItemsCollector.ICollectToDestinationMarker> collectToDestinationExecutionStrategy,
    IAsyncExecutionStrategy<CollectItemsCollector.ICollectItemsMarker> collectItemsExecutionStrategy,
    IAsyncExecutionStrategy<CollectItemsCollector.IGarbageCollectTargetsMarker> garbageCollectTargetsExecutionStrategy) : ICollectItemsCollector
{
    public interface ICollectToDestinationMarker { }
    public interface ICollectItemsMarker { }
    public interface IGarbageCollectTargetsMarker { }

    private static readonly string[] _retryableErrorMessages = ["Gateway Timeout 504", "The request timed out"];

    public static bool IsRetryableCollectError(Exception e)
    {
        return _retryableErrorMessages.Any(x => e.Message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<IEnumerable<string>> CollectItemsAsync(
        IEnumerable<CollectItem> collectItems,
        string dataCollectionName,
        IEnumerable<IDestination> destinations,
        DataCollectionConfig dataCollectionConfig,
        DateTimeOffset collectMoment,
        CancellationToken cancellationToken)
    {
        var routes = collectItems.Select(x => (CollectItem: x, Targets: destinations.Select(y => (Destination: y, DestinationFileName: GetDestinationFileName(x, dataCollectionConfig, y, collectMoment)))));

        var lockTargets = GetLockTargets(routes.SelectMany(x => x.Targets));

        if (dataCollectionConfig.ParallelDestinationCount > 1)
        {
            routes = routes.SelectMany(x => x.Targets.Select(y => (CollectItem: x.CollectItem, Targets: (IEnumerable<(IDestination Destination, string DestinationFileName)>)([(y.Destination, y.DestinationFileName)]))));
        }

        var completeDestinationFiles = new ConcurrentBag<(IDestination Destination, string FileName, string GroupId)>();
        var failedDestinationGroups = new ConcurrentBag<(IDestination Destination, string GroupId)>();

        await CollectRouteAsync(new CollectItem(null, string.Empty), lockTargets, dataCollectionConfig, completeDestinationFiles, failedDestinationGroups, cancellationToken);

        await routes.ParallelForEachAsync(
            async (route, ct2) =>
                await collectItemsExecutionStrategy.ExecuteAsync(
                    async (ct) => await CollectRouteAsync(route.CollectItem, route.Targets, dataCollectionConfig, completeDestinationFiles, failedDestinationGroups, ct),
                    ct2),
            Math.Max(1, dataCollectionConfig.ParallelDestinationCount ?? 0),
            cancellationToken);

        if (!failedDestinationGroups.IsEmpty)
        {
            await GarbageCollectFailedDestinationsAsync(failedDestinationGroups, completeDestinationFiles, dataCollectionConfig, cancellationToken);

            var failedDestinationNames = string.Join(", ", failedDestinationGroups.Select(x => x.Destination.DestinationConfig.DestinationId).Distinct());

            throw new Exception($"Failed to collect data to destinations '{failedDestinationNames}'");
        }

        await GarbageCollectTargetsAsync(
            lockTargets.GroupBy(x => x.Destination).Select(x => (Destination: x.Key, DestinationFileNames: x.Select(y => y.DestinationFileName))),
            dataCollectionConfig,
            cancellationToken);

        return completeDestinationFiles.Select(x => x.FileName).Distinct();
    }

    private string GetDestinationFileName(CollectItem collectItem, DataCollectionConfig dataCollectionConfig, IDestination destination, DateTimeOffset collectMoment)
    {
        return
            fileNameProvider.GenerateCollectDestinationFileName(
                dataCollectionConfig.DataCollectionName,
                collectItem.CollectFileInfo?.Name,
                collectItem.CollectUrl,
                collectMoment,
                destination.DestinationConfig.CollectToDirectories,
                destination.DestinationConfig.GenerateFileNames);
    }

    private IEnumerable<(IDestination Destination, string DestinationFileName)> GetLockTargets(IEnumerable<(IDestination Destination, string DestinationFileName)> targets)
    {
        return
            targets
            .Select(x => (x.Destination, DestinationDirName: string.Join("/", x.DestinationFileName.Split("/").SkipLast(1))))
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => (x.Key.Destination, DestinationFileName: x.Key.DestinationDirName + "/" + fileNameProvider.LockFileName));
    }

    private async Task CollectRouteAsync(
        CollectItem collectItem,
        IEnumerable<(IDestination Destination, string DestinationFileName)> targets,
        DataCollectionConfig dataCollectionConfig,
        ConcurrentBag<(IDestination Destination, string FileName, string GroupId)> completeDestinationFiles,
        ConcurrentBag<(IDestination Destination, string GroupId)> failedDestinationGroups,
        CancellationToken cancellationToken)
    {
        foreach (var target in targets)
        {
            try
            {
                if (!failedDestinationGroups.Contains((target.Destination, collectItem.CollectFileInfo?.GroupId ?? "default")))
                {
                    await CollectToDestinationAsync(
                        collectItem, target.Destination, target.DestinationFileName, dataCollectionConfig, completeDestinationFiles, cancellationToken);
                }
            }
            catch
            {
                failedDestinationGroups.Add((target.Destination, collectItem.CollectFileInfo?.GroupId ?? "default"));
                // do not throw exception, give other destinations a chance
            }
        }
    }

    private async Task CollectToDestinationAsync(
        CollectItem collectItem,
        IDestination destination,
        string destinationFileName,
        DataCollectionConfig dataCollectionConfig,
        ConcurrentBag<(IDestination Destination, string FileName, string GroupId)> completeDestinationFiles,
        CancellationToken cancellationToken)
    {
        await collectToDestinationExecutionStrategy.ExecuteAsync(
            async (ct) =>
            {
                if (string.IsNullOrEmpty(collectItem.CollectUrl))
                {
                    await destination.PutFileAsync(dataCollectionConfig.DataCollectionName, destinationFileName, new MemoryStream(), ct);
                }
                else
                {
                    await destination.CollectAsync(
                        collectItem.CollectUrl,
                        dataCollectionConfig.CollectHeaders,
                        dataCollectionConfig.IdentityServiceClientInfo,
                        dataCollectionConfig.DataCollectionName,
                        destinationFileName, dataCollectionConfig.CollectTimeout ?? default,
                        (dataCollectionConfig.CollectFinishWait ?? "false") == "true",
                        ct);
                }
            },
            cancellationToken);

        completeDestinationFiles.Add((destination, destinationFileName, collectItem.CollectFileInfo?.GroupId ?? "default"));
    }

    private async Task GarbageCollectFailedDestinationsAsync(
        IEnumerable<(IDestination Destination, string GroupId)> failedDestinationGroups,
        IEnumerable<(IDestination Destination, string FileName, string GroupId)> completeDestinationFiles,
        DataCollectionConfig dataCollectionConfig,
        CancellationToken cancellationToken)
    {
        var failedTargets =
                failedDestinationGroups
                .Distinct()
                .Select(x => (Destination: x.Destination, DestinationFileNames: completeDestinationFiles.Where(y => (y.Destination == x.Destination) && (y.GroupId == x.GroupId)).Select(y => y.FileName)))
                .GroupBy(x => x.Destination)
                .Select(x => (Destination: x.Key, DestinationFileNames: x.SelectMany(y => y.DestinationFileNames)));

        await GarbageCollectTargetsAsync(failedTargets, dataCollectionConfig, cancellationToken);
    }

    private async Task GarbageCollectTargetsAsync(
        IEnumerable<(IDestination Destination, IEnumerable<string> DestinationFileNames)> targets,
        DataCollectionConfig dataCollectionConfig,
        CancellationToken cancellationToken)
    {
        await targets.ParallelForEachAsync(
            async (target, ct2) =>
                await garbageCollectTargetsExecutionStrategy.ExecuteAsync(
                    async (ct) => await GarbageCollectDestinationFilesAsync(target.Destination, dataCollectionConfig.DataCollectionName, target.DestinationFileNames, ct),
                    ct2),
                Math.Max(1, dataCollectionConfig.ParallelDestinationCount ?? 1),
                cancellationToken);
    }

    private static async Task GarbageCollectDestinationFilesAsync(
        IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
    {
        await Task.WhenAll(fileNames.Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
    }
}
