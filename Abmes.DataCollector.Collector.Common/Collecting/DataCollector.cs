using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;
using System.Collections.Concurrent;
using Polly;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class DataCollector : IDataCollector
    {
        private readonly IDestinationProvider _destinationProvider;
        private readonly IDataPreparer _dataPreparer;
        private readonly ICollectItemsProvider _collectItemsProvider;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IDelay _delay;
        private readonly ICollectItemsCollector _collectItemsCollector;

        public DataCollector(
            IDestinationProvider destinationProvider,
            IDataPreparer DataPreparer,
            ICollectItemsProvider collectItemsProvider,
            IFileNameProvider fileNameProvider,
            IDelay delay,
            ICollectItemsCollector collectItemsCollector)
        {
            _destinationProvider = destinationProvider;
            _dataPreparer = DataPreparer;
            _collectItemsProvider = collectItemsProvider;
            _fileNameProvider = fileNameProvider;
            _delay = delay;
            _collectItemsCollector = collectItemsCollector;
        }

        public async Task<(IEnumerable<string> NewFileNames, IEnumerable<IFileInfoData> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            // assert at least one destination before preparing
            var destinations = await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken);

            if (!destinations.Any())
            {
                throw new Exception("No destinations found");
            }

            if ((dataCollectionConfig.InitialDelay ?? default).TotalSeconds > 0)
            {
                await _delay.DelayAsync(dataCollectionConfig.InitialDelay ?? default, $"initial delay for Data '{dataCollectionConfig.DataCollectionName}'", cancellationToken);
            }

            var collectMoment = DateTimeOffset.Now;

            var prepared = (collectorMode != CollectorMode.Collect) ? false : await _dataPreparer.PrepareDataAsync(dataCollectionConfig, cancellationToken);

            var collectItems = _collectItemsProvider.GetCollectItems(dataCollectionConfig.DataCollectionName, dataCollectionConfig.CollectFileIdentifiersUrl, dataCollectionConfig.CollectFileIdentifiersHeaders, dataCollectionConfig.CollectUrl, dataCollectionConfig.CollectHeaders, dataCollectionConfig.IdentityServiceClientInfo, cancellationToken).ToList();
            var collectionFileInfos = collectItems.Select(x => x.CollectFileInfo);

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

                var redirectedCollectItems = await _collectItemsProvider.GetRedirectedCollectItemsAsync(acceptedCollectItems, dataCollectionConfig.DataCollectionName, dataCollectionConfig.CollectHeaders, dataCollectionConfig.CollectParallelFileCount ?? 1, dataCollectionConfig.IdentityServiceClientInfo, cancellationToken);

                var newFileNames = await _collectItemsCollector.CollectItemsAsync(redirectedCollectItems, dataCollectionConfig.DataCollectionName, destinations, dataCollectionConfig, collectMoment, cancellationToken);

                return (newFileNames, collectionFileInfos);
            }

            if ((collectorMode == CollectorMode.Check) && acceptedCollectItems.Any())
            {
                throw new Exception("Found missing data");
            }

            return (Enumerable.Empty<string>(), collectionFileInfos);
        }

        private async Task<IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)>> GetAcceptedCollectItemsAsync(IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, int maxDegreeOfParallelism, CancellationToken cancellationToken)
        {
            var result = new ConcurrentBag<(IFileInfoData CollectFileInfo, string CollectUrl)>();

            await ParallelUtils.ParallelEnumerateAsync(collectItems, cancellationToken, Math.Max(1, maxDegreeOfParallelism),
                async (collectItem, ct) =>
                {
                    if (collectItem.CollectFileInfo == null)
                    {
                        result.Add(collectItem);
                    }
                    else
                    {
                        foreach (var destination in destinations)
                        {
                            if (await destination.AcceptsFileAsync(dataCollectionName, collectItem.CollectFileInfo.Name, collectItem.CollectFileInfo.Size, collectItem.CollectFileInfo.MD5, cancellationToken))
                            {
                                result.Add(collectItem);
                                break;
                            }
                        }
                    }
                });

            return result;
        }

        public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<IFileInfoData> collectionFileInfos, CancellationToken cancellationToken)
        {
            var destinations = (await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken)).Where(x => x.CanGarbageCollect());
            await Task.WhenAll(destinations.Select(x => GarbageCollectDestinationDataAsync(x, dataCollectionConfig.DataCollectionName, newFileNames, collectionFileInfos, cancellationToken)));
        }

        private async Task GarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, IEnumerable<string> newFileNames, IEnumerable<IFileInfoData> collectionFileInfos, CancellationToken cancellationToken)
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
                    await ExcessGarbageCollectDestinationDataAsync(destination, dataCollectionName, newFileNames, collectionFileInfos, cancellationToken);
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

        private async Task ExcessGarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, IEnumerable<string> newFileNames, IEnumerable<IFileInfoData> collectionFileInfos, CancellationToken cancellationToken)
        {
            var destinationDataCollectionFileNames = (await destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken)).ToList();

            var collectionFileNames = collectionFileInfos.Select(x => x.Name).OrderBy(x => x).ToList();

            var garbageDataCollectionFileNames =
                    destinationDataCollectionFileNames
                    .Where(x => !collectionFileNames.Contains(x, StringComparer.InvariantCultureIgnoreCase));

            await GarbageCollectDestinationFilesAsync(destination, dataCollectionName, garbageDataCollectionFileNames, cancellationToken);
        }

        private async Task GarbageCollectDestinationFilesAsync(IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
        {
            await Task.WhenAll(fileNames.OrderBy(x => x).Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
        }

        private async Task<IEnumerable<IDestination>> GetDestinationsAsync(IEnumerable<string> destinationIds, CancellationToken cancellationToken)
        {
            return await Task.WhenAll(destinationIds.Select(x => _destinationProvider.GetDestinationAsync(x, cancellationToken)));
        }

        private IEnumerable<string> GetWaterfallGarbageDataCollectionFileNames(IEnumerable<string> dataCollectionFileNames, IEnumerable<string> newFileNames)
        {
            var now = DateTimeOffset.UtcNow;

            var files =
                    dataCollectionFileNames
                    .Select(x => new { FileName = x, FileDateTime = _fileNameProvider.DataCollectionFileNameToDateTime(x), IsNew = newFileNames.Contains(x) })
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
                    .Where(x => x.RelativeDateInfo.RelativeDayNo.InRange(1, 6))
                    .GroupBy(x => x.RelativeDateInfo.RelativeDayNo)
                    .SelectMany(x => x.OrderByDescending(y => y.IsNew).ThenBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

            var weeklyFileNames =
                    files
                    .Where(x => x.RelativeDateInfo.RelativeWeekNo.InRange(1, 4))
                    .GroupBy(x => x.RelativeDateInfo.RelativeWeekNo)
                    .SelectMany(x => x.OrderByDescending(y => y.IsNew).ThenBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

            var monthlyFileNames =
                    files
                    .Where(x => x.RelativeDateInfo.RelativeMonthNo.InRange(1, 3))
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
}
