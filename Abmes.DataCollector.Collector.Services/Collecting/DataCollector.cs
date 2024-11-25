using Abmes.DataCollector.Collector.Data.Configuration;
using Abmes.DataCollector.Collector.Data.Destinations;
using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Contracts;
using Abmes.DataCollector.Collector.Services.Misc;
using Abmes.DataCollector.Common;
using Abmes.DataCollector.Common.Services.Storage;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public class DataCollector(
    IDestinationProvider destinationProvider,
    IDataPreparer dataPreparer,
    ICollectItemsProvider collectItemsProvider,
    IFileNameProvider fileNameProvider,
    IDelay delay,
    TimeProvider timeProvider,
    IAsyncExecutionStrategy<DataCollector> executionStrategy,
    ICollectItemsCollector collectItemsCollector) : IDataCollector
{
    public async Task<(IEnumerable<string> NewFileNames, IEnumerable<FileInfoData> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
    {
        // assert at least one destination before preparing
        var destinations = await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken);

        if (!destinations.Any())
        {
            throw new Exception("No destinations found");
        }

        if ((dataCollectionConfig.InitialDelay ?? default).TotalSeconds > 0)
        {
            await delay.DelayAsync(dataCollectionConfig.InitialDelay ?? default, $"initial delay for Data '{dataCollectionConfig.DataCollectionName}'", cancellationToken);
        }

        var collectMoment = timeProvider.GetUtcNow();

        var prepared = collectorMode == CollectorMode.Collect && await dataPreparer.PrepareDataAsync(dataCollectionConfig, cancellationToken);

        ArgumentException.ThrowIfNullOrEmpty(dataCollectionConfig.CollectUrl);

        var collectItems =
            (await collectItemsProvider.GetCollectItemsAsync(
                dataCollectionConfig.DataCollectionName,
                dataCollectionConfig.CollectFileIdentifiersUrl,
                dataCollectionConfig.CollectFileIdentifiersHeaders,
                dataCollectionConfig.CollectUrl,
                dataCollectionConfig.CollectHeaders,
                dataCollectionConfig.IdentityServiceClientInfo,
                cancellationToken))
            .ToList();

        var collectionFileInfos = collectItems.Where(x => x.CollectFileInfo is not null).Select(x => Ensure.NotNull(x.CollectFileInfo));

        var acceptedCollectItems = await GetAcceptedCollectItemsAsync(collectItems, dataCollectionConfig.DataCollectionName, destinations, dataCollectionConfig.CollectParallelFileCount ?? 1, cancellationToken);

        if (collectorMode == CollectorMode.Collect)
        {
            if (prepared && (!acceptedCollectItems.Any()))
            {
                throw new Exception("No data prepared for collecting");
            }

            if (dataCollectionConfig.MaxFileCount.HasValue)
            {
                acceptedCollectItems = acceptedCollectItems.Take(dataCollectionConfig.MaxFileCount.Value);
            }

            var redirectedCollectItems =
                await collectItemsProvider.GetRedirectedCollectItemsAsync(
                    acceptedCollectItems,
                    dataCollectionConfig.DataCollectionName,
                    dataCollectionConfig.CollectHeaders,
                    dataCollectionConfig.CollectParallelFileCount ?? 1,
                    dataCollectionConfig.IdentityServiceClientInfo,
                    cancellationToken);

            var newFileNames = await collectItemsCollector.CollectItemsAsync(redirectedCollectItems, dataCollectionConfig.DataCollectionName, destinations, dataCollectionConfig, collectMoment, cancellationToken);

            return (newFileNames, collectionFileInfos);
        }

        return
            (collectorMode == CollectorMode.Check) && acceptedCollectItems.Any()
            ? throw new Exception("Found missing data")
            : ([], collectionFileInfos);
    }

    private async Task<IEnumerable<CollectItem>> GetAcceptedCollectItemsAsync(IEnumerable<CollectItem> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, int maxDegreeOfParallelism, CancellationToken cancellationToken)
    {
        var result =
            await collectItems.ParallelForEachAsync(
                async (collectItem, ct2) =>
                    await executionStrategy.ExecuteAsync(
                        async (ct) =>
                        {
                            if (collectItem.CollectFileInfo is null)
                            {
                                return collectItem;
                            }

                            foreach (var destination in destinations)
                            {
                                if (await destination.AcceptsFileAsync(dataCollectionName, collectItem.CollectFileInfo.Name, collectItem.CollectFileInfo.Size, collectItem.CollectFileInfo.MD5, ct))
                                {
                                    return collectItem;
                                }
                            }

                            return null;
                        },
                        ct2),
                Math.Max(1, maxDegreeOfParallelism),
                cancellationToken);

        return
            result
            .Where(collectItem => collectItem is not null)
            .Select(collectItem => Ensure.NotNull(collectItem));
    }

    public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<FileInfoData> collectionFileInfos, CancellationToken cancellationToken)
    {
        var destinations = (await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken)).Where(x => x.CanGarbageCollect());
        await Task.WhenAll(destinations.Select(x => GarbageCollectDestinationDataAsync(x, dataCollectionConfig.DataCollectionName, newFileNames, collectionFileInfos, cancellationToken)));
    }

    private async Task GarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, IEnumerable<string> newFileNames, IEnumerable<FileInfoData> collectionFileInfos, CancellationToken cancellationToken)
    {
        if (!destination.DestinationConfig.GarbageCollectionMode.HasValue)
        {
            return;
        }

        switch (destination.DestinationConfig.GarbageCollectionMode.Value)
        {
            case GarbageCollectionMode.None:
                break;

            case GarbageCollectionMode.Waterfall:
                await WaterfallGarbageCollectDestinationDataAsync(destination, dataCollectionName, newFileNames, cancellationToken);
                break;

            case GarbageCollectionMode.Excess:
                await ExcessGarbageCollectDestinationDataAsync(destination, dataCollectionName, collectionFileInfos, cancellationToken);
                break;

            default:
                throw new Exception($"Unknown GarbageCollectionMode: {destination.DestinationConfig.GarbageCollectionMode}");
        }
    }

    private async Task WaterfallGarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, IEnumerable<string> newFileNames, CancellationToken cancellationToken)
    {
        var destinationDataCollectionFileNames = await destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);
        var garbageDataCollectionFileNames = GetWaterfallGarbageDataCollectionFileNames(destinationDataCollectionFileNames, newFileNames);

        await GarbageCollectDestinationFilesAsync(destination, dataCollectionName, garbageDataCollectionFileNames, cancellationToken);
    }

    private static async Task ExcessGarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, IEnumerable<FileInfoData> collectionFileInfos, CancellationToken cancellationToken)
    {
        var destinationDataCollectionFileNames = (await destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken)).ToList();

        var collectionFileNames = collectionFileInfos.Select(x => x.Name).OrderBy(x => x).ToList();

        var garbageDataCollectionFileNames =
                destinationDataCollectionFileNames
                .Where(x => !collectionFileNames.Contains(x, StringComparer.InvariantCultureIgnoreCase));

        await GarbageCollectDestinationFilesAsync(destination, dataCollectionName, garbageDataCollectionFileNames, cancellationToken);
    }

    private static async Task GarbageCollectDestinationFilesAsync(IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
    {
        await Task.WhenAll(fileNames.OrderBy(x => x).Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
    }

    private async Task<IEnumerable<IDestination>> GetDestinationsAsync(IEnumerable<string> destinationIds, CancellationToken cancellationToken)
    {
        return await Task.WhenAll(destinationIds.Select(x => destinationProvider.GetDestinationAsync(x, cancellationToken)));
    }

    private IEnumerable<string> GetWaterfallGarbageDataCollectionFileNames(IEnumerable<string> dataCollectionFileNames, IEnumerable<string> newFileNames)
    {
        var now = timeProvider.GetUtcNow();

        var files =
                dataCollectionFileNames
                .Select(x => new { FileName = x, FileDateTime = fileNameProvider.DataCollectionFileNameToDateTime(x), IsNew = newFileNames.Contains(x) })
                .GroupBy(x => x.FileDateTime)
                .Select(x => new { FileNames = x.Select(y => y.FileName), FilesDateTime = x.Key, IsNew = !x.Any(y => !y.IsNew) })
                .Select(x => new { x.FileNames, x.FilesDateTime, RelativeDateInfo = new RelativeDateInfo(x.FilesDateTime.Date, now.Date), x.IsNew })
                .ToList();

        var hourlyFileNames =
                files
                .Where(x => (now - x.FilesDateTime).TotalHours <= 24)
                .OrderByDescending(x => x.IsNew).ThenBy(x => x.FilesDateTime)
                .SelectMany(x => x.FileNames);

        var dailyFileNames =
                files
                .Where(x => x.RelativeDateInfo.RelativeDayNo is >= 1 and <= 6)
                .GroupBy(x => x.RelativeDateInfo.RelativeDayNo)
                .SelectMany(x => x.OrderByDescending(y => y.IsNew).ThenBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

        var weeklyFileNames =
                files
                .Where(x => x.RelativeDateInfo.RelativeWeekNo is >= 1 and <= 4)
                .GroupBy(x => x.RelativeDateInfo.RelativeWeekNo)
                .SelectMany(x => x.OrderByDescending(y => y.IsNew).ThenBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

        var monthlyFileNames =
                files
                .Where(x => x.RelativeDateInfo.RelativeMonthNo is >= 1 and <= 3)
                .GroupBy(x => x.RelativeDateInfo.RelativeMonthNo)
                .SelectMany(x => x.OrderByDescending(y => y.IsNew).ThenBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

        var quaterlyFileNames =
                files
                .Where(x => x.RelativeDateInfo.RelativeQuarterNo > 1)
                .GroupBy(x => x.RelativeDateInfo.RelativeQuarterNo)
                .SelectMany(x => x.OrderByDescending(y => y.IsNew).ThenBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

        //foreach (var fn in hourlyFileNames) Console.WriteLine("Preserving hourly file: " + fn);
        //foreach (var fn in dailyFileNames) Console.WriteLine("Preserving daily file: " + fn);
        //foreach (var fn in weeklyFileNames) Console.WriteLine("Preserving weekly file: " + fn);
        //foreach (var fn in monthlyFileNames) Console.WriteLine("Preserving monthly file: " + fn);
        //foreach (var fn in quaterlyFileNames) Console.WriteLine("Preserving houquaterlyrly file: " + fn);

        var preserveFileNames =
                hourlyFileNames
                .Union(dailyFileNames)
                .Union(weeklyFileNames)
                .Union(monthlyFileNames)
                .Union(quaterlyFileNames)
                .Distinct();

        return dataCollectionFileNames.Except(preserveFileNames);
    }
}
