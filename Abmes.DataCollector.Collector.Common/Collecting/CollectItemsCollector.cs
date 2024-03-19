using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Polly;
using System.Collections.Concurrent;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public class CollectItemsCollector(
    IFileNameProvider fileNameProvider) : ICollectItemsCollector
{
    public async Task<IEnumerable<string>> CollectItemsAsync(
        IEnumerable<(FileInfoData? CollectFileInfo, string CollectUrl)> collectItems,
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

        await CollectRouteAsync(default, lockTargets, dataCollectionConfig, completeDestinationFiles, failedDestinationGroups, cancellationToken);

        await ParallelUtils.ParallelEnumerateAsync(
            routes,
            Math.Max(1, dataCollectionConfig.ParallelDestinationCount ?? 0),
            (route, ct) => CollectRouteAsync(route.CollectItem, route.Targets, dataCollectionConfig, completeDestinationFiles, failedDestinationGroups, ct),
            cancellationToken);

        if (failedDestinationGroups.Any())
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

    private string GetDestinationFileName((FileInfoData? CollectFileInfo, string CollectUrl) collectItem, DataCollectionConfig dataCollectionConfig, IDestination destination, DateTimeOffset collectMoment)
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

    private static async Task CollectRouteAsync(
        (FileInfoData? CollectFileInfo, string CollectUrl) collectItem,
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

    private static async Task CollectToDestinationAsync(
        (FileInfoData? CollectFileInfo, string CollectUrl) collectItem,
        IDestination destination,
        string destinationFileName,
        DataCollectionConfig dataCollectionConfig,
        ConcurrentBag<(IDestination Destination, string FileName, string GroupId)> completeDestinationFiles,
        CancellationToken cancellationToken)
    {
        var tryNo = 1;
        await Policy
            .Handle<Exception>(e => IsRetryableCollectError(e))
            .WaitAndRetryAsync(
                2,
                (x) => TimeSpan.FromSeconds(5),
                (exception, timeSpan) => { tryNo++; })
            .ExecuteAsync(
                async (ct) =>
                {
                    if (string.IsNullOrEmpty(collectItem.CollectUrl))
                    {
                        await destination.PutFileAsync(dataCollectionConfig.DataCollectionName, destinationFileName, new MemoryStream(), cancellationToken);
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
                            tryNo,
                            ct);
                    }
                },
                cancellationToken);

        completeDestinationFiles.Add((destination, destinationFileName, collectItem.CollectFileInfo?.GroupId ?? "default"));
    }

    private static readonly string[] _retryableErrorMessages = ["Gateway Timeout 504", "The request timed out"];

    private static bool IsRetryableCollectError(Exception e)
    {
        return _retryableErrorMessages.Any(x => e.Message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }

    private static async Task GarbageCollectFailedDestinationsAsync(
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

    private static async Task GarbageCollectTargetsAsync(
        IEnumerable<(IDestination Destination, IEnumerable<string> DestinationFileNames)> targets,
        DataCollectionConfig dataCollectionConfig,
        CancellationToken cancellationToken)
    {
        await ParallelUtils.ParallelEnumerateAsync(
            targets,
            Math.Max(1, dataCollectionConfig.ParallelDestinationCount ?? 1),
            (target, ct) => GarbageCollectDestinationFilesAsync(target.Destination, dataCollectionConfig.DataCollectionName, target.DestinationFileNames, ct),
            cancellationToken);
    }

    private static async Task GarbageCollectDestinationFilesAsync(
        IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
    {
        await Task.WhenAll(fileNames.Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
    }
}
