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

        public async Task CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            // assert at least one destination before preparing
            var destinations = await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken);

            if (!destinations.Any())
            {
                throw new Exception("No destinations found");
            }

            if (dataCollectionConfig.InitialDelay.TotalSeconds > 0)
            {
                await _delay.DelayAsync(dataCollectionConfig.InitialDelay, $"initial delay for Data '{dataCollectionConfig.DataCollectionName}'", cancellationToken);
            }

            var collectMoment = DateTimeOffset.Now;

            var prepared = (collectorMode != CollectorMode.Collect) ? false : await _dataPreparer.PrepareDataAsync(dataCollectionConfig, cancellationToken);

            var collectItems = _collectItemsProvider.GetCollectItems(dataCollectionConfig.DataCollectionName, dataCollectionConfig.CollectFileIdentifiersUrl, dataCollectionConfig.CollectFileIdentifiersHeaders, dataCollectionConfig.CollectUrl, dataCollectionConfig.IdentityServiceClientInfo, cancellationToken);

            var acceptedCollectItems = await GetAcceptedCollectItemsAsync(collectItems, dataCollectionConfig.DataCollectionName, destinations, dataCollectionConfig.CollectParallelFileCount, cancellationToken);

            if (collectorMode == CollectorMode.Collect)
            {
                if (prepared && (!acceptedCollectItems.Any()))
                {
                    throw new Exception("No data to collect");
                }

                var redirectedCollectItems = await _collectItemsProvider.GetRedirectedCollectItemsAsync(acceptedCollectItems, dataCollectionConfig.DataCollectionName, dataCollectionConfig.CollectHeaders, dataCollectionConfig.CollectParallelFileCount, dataCollectionConfig.IdentityServiceClientInfo, cancellationToken);

                await _collectItemsCollector.CollectItemsAsync(redirectedCollectItems, dataCollectionConfig.DataCollectionName, destinations, dataCollectionConfig, collectMoment, cancellationToken);
            }

            if ((collectorMode == CollectorMode.Check) && acceptedCollectItems.Any())
            {
                throw new Exception("Found missing data");
            }
        }

        private async Task<IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)>> GetAcceptedCollectItemsAsync(IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, int maxDegreeOfParallelism, CancellationToken cancellationToken)
        {
            var result = new List<(IFileInfo CollectFileInfo, string CollectUrl)>();

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

        public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            var destinations = (await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken)).Where(x => x.CanGarbageCollect());
            await Task.WhenAll(destinations.Select(x => GarbageCollectDestinationDataAsync(x, dataCollectionConfig.DataCollectionName, cancellationToken)));
        }

        private async Task GarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, CancellationToken cancellationToken)
        {
            var dataCollectionFileNames = await destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);
            var garbageDataCollectionFileNames = GetGarbageDataCollectionFileNames(dataCollectionFileNames);

            await GarbageCollectDestinationFilesAsync(destination, dataCollectionName, garbageDataCollectionFileNames, cancellationToken);
        }

        private async Task GarbageCollectDestinationFilesAsync(IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
        {
            await Task.WhenAll(fileNames.Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
        }

        private async Task<IEnumerable<IDestination>> GetDestinationsAsync(IEnumerable<string> destinationIds, CancellationToken cancellationToken)
        {
            return await Task.WhenAll(destinationIds.Select(x => _destinationProvider.GetDestinationAsync(x, cancellationToken)));
        }

        private IEnumerable<string> GetGarbageDataCollectionFileNames(IEnumerable<string> dataCollectionFileNames)
        {
            var now = DateTimeOffset.UtcNow;

            var files =
                    dataCollectionFileNames
                    .Select(x => new { FileName = x, FileDateTime = _fileNameProvider.DataCollectionFileNameToDateTime(x) })
                    .GroupBy(x => x.FileDateTime)
                    .Select(x => new { FileNames = x.Select(y => y.FileName), FilesDateTime = x.Key })
                    .Select(x => new { x.FileNames, x.FilesDateTime, RelativeDateInfo = new RelativeDateInfo(x.FilesDateTime.Date, now.Date) })
                    .ToList();

            var hourlyFileNames =
                    files
                    .Where(x => (now - x.FilesDateTime).TotalHours <= 24)
                    .OrderBy(x => x.FilesDateTime)
                    .SelectMany(x => x.FileNames);

            var dailyFileNames =
                    files
                    .Where(x => x.RelativeDateInfo.RelativeDayNo.InRange(1, 6))
                    .GroupBy(x => x.RelativeDateInfo.RelativeDayNo)
                    .SelectMany(x => x.OrderBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

            var weeklyFileNames =
                    files
                    .Where(x => x.RelativeDateInfo.RelativeWeekNo.InRange(1, 4))
                    .GroupBy(x => x.RelativeDateInfo.RelativeWeekNo)
                    .SelectMany(x => x.OrderBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

            var monthlyFileNames =
                    files
                    .Where(x => x.RelativeDateInfo.RelativeMonthNo.InRange(1, 3))
                    .GroupBy(x => x.RelativeDateInfo.RelativeMonthNo)
                    .SelectMany(x => x.OrderBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

            var quaterlyFileNames =
                    files
                    .Where(x => x.RelativeDateInfo.RelativeQuarterNo > 1)
                    .GroupBy(x => x.RelativeDateInfo.RelativeQuarterNo)
                    .SelectMany(x => x.OrderBy(y => y.FilesDateTime).Select(y => y.FileNames).First());

            foreach (var fn in hourlyFileNames) Console.WriteLine("Preserving hourly file: " + fn);
            foreach (var fn in dailyFileNames) Console.WriteLine("Preserving daily file: " + fn);
            foreach (var fn in weeklyFileNames) Console.WriteLine("Preserving weekly file: " + fn);
            foreach (var fn in monthlyFileNames) Console.WriteLine("Preserving monthly file: " + fn);
            foreach (var fn in quaterlyFileNames) Console.WriteLine("Preserving houquaterlyrly file: " + fn);

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
