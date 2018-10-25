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

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class DataCollector : IDataCollector
    {
        private readonly IDestinationProvider _destinationProvider;
        private readonly IDataPreparer _dataPreparer;
        private readonly ICollectUrlsProvider _collectUrlsProvider;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IDelay _delay;

        public DataCollector(
            IDestinationProvider destinationProvider,
            IDataPreparer DataPreparer,
            ICollectUrlsProvider collectUrlsProvider,
            IFileNameProvider fileNameProvider,
            IDelay delay)
        {
            _destinationProvider = destinationProvider;
            _dataPreparer = DataPreparer;
            _collectUrlsProvider = collectUrlsProvider;
            _fileNameProvider = fileNameProvider;
            _delay = delay;
        }

        public async Task CollectDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken)
        {
            // assert at least one destination before preparing
            var destinations = await GetDestinationsAsync(DataCollectConfig.DestinationIds, cancellationToken);

            if (!destinations.Any())
            {
                throw new Exception("No destinations found");
            }

            if (DataCollectConfig.InitialDelay.TotalSeconds > 0)
            {
                await _delay.DelayAsync(DataCollectConfig.InitialDelay, $"initial delay for Data '{DataCollectConfig.DataCollectionName}'", cancellationToken);
            }

            var collectMoment = DateTimeOffset.Now;

            await _dataPreparer.PrepareDataAsync(DataCollectConfig, cancellationToken);

            var collectUrls = _collectUrlsProvider.GetCollectUrls(DataCollectConfig.CollectFileIdentifiersUrl, DataCollectConfig.CollectFileIdentifiersHeaders, DataCollectConfig.CollectUrl).ToList();

            if (!collectUrls.Any())
            {
                throw new Exception($"No files to collect for Data '{DataCollectConfig.DataCollectionName}'");
            }

            foreach (var destination in destinations)
            {
                foreach (var collectUrl in collectUrls)
                {
                    var destinationFileName =
                          _fileNameProvider.GenerateCollectDestinationFileName(
                              DataCollectConfig.DataCollectionName,
                              collectUrl,
                              collectMoment,
                              !string.IsNullOrEmpty(DataCollectConfig.CollectFileIdentifiersUrl)
                            );

                    await destination.CollectAsync(collectUrl, DataCollectConfig.CollectHeaders, DataCollectConfig.DataCollectionName, destinationFileName, DataCollectConfig.CollectTimeout, DataCollectConfig.CollectFinishWait, cancellationToken);
                }
            }
        }

        public async Task GarbageCollectDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken)
        {
            var destinations = await GetDestinationsAsync(DataCollectConfig.DestinationIds, cancellationToken);
            await Task.WhenAll(destinations.Select(x => GarbageCollectDestinationDataAsync(x, DataCollectConfig.DataCollectionName, cancellationToken)));
        }

        private async Task GarbageCollectDestinationDataAsync(IDestination destination, string dataCollectionName, CancellationToken cancellationToken)
        {
            var DataCollectionFileNames = await destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);
            var garbageDataCollectionFileNames = GetGarbageDataCollectionFileNames(DataCollectionFileNames);

            await Task.WhenAll(garbageDataCollectionFileNames.Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
        }
    
        private async Task<IEnumerable<IDestination>> GetDestinationsAsync(IEnumerable<string> destinationIds, CancellationToken cancellationToken)
        {
            return await Task.WhenAll(destinationIds.Select(x => _destinationProvider.GetDestinationAsync(x, cancellationToken)));
        }

        private IEnumerable<string> GetGarbageDataCollectionFileNames(IEnumerable<string> DataCollectionFileNames)
        {
            var now = DateTimeOffset.UtcNow;

            var files =
                    DataCollectionFileNames
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

            var preserveFileNames =
                    hourlyFileNames
                    .Union(dailyFileNames)
                    .Union(weeklyFileNames)
                    .Union(monthlyFileNames)
                    .Union(quaterlyFileNames)
                    .Distinct();

            return DataCollectionFileNames.Except(preserveFileNames);
        }
    }
}
