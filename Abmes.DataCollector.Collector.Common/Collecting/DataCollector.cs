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

        public async Task CollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
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

            await _dataPreparer.PrepareDataAsync(dataCollectionConfig, cancellationToken);

            var collectUrls = await _collectUrlsProvider.GetCollectUrlsAsync(dataCollectionConfig.DataCollectionName, dataCollectionConfig.CollectFileIdentifiersUrl, dataCollectionConfig.CollectFileIdentifiersHeaders, dataCollectionConfig.CollectUrl, dataCollectionConfig.CollectHeaders, dataCollectionConfig.CollectParallelFileCount, cancellationToken);

            await CollectUrlsAsync(collectUrls, destinations, dataCollectionConfig, collectMoment, cancellationToken);
        }

        private async Task CollectUrlsAsync(IEnumerable<string> collectUrls, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken)
        {
            var routes = collectUrls.Select(x => (CollectUrl: x, Destinations: destinations));

            if (dataCollectionConfig.ParallelDestinations)
            {
                routes = routes.SelectMany(x => x.Destinations.Select(y => (CollectUrl: x.CollectUrl, Destinations: (IEnumerable<IDestination>)(new[] { y }))));
            }

            var completeFileNames = new ConcurrentBag<(IDestination Destination, string FileName)>();
            var failedDestinations = new ConcurrentBag<IDestination>();

            var workerBlock =
                    await ParallelUtils.ParallelEnumerateAsync(routes, cancellationToken, Math.Max(1, dataCollectionConfig.CollectParallelFileCount),
                    (route, ct) => CollectRouteAsync(route, dataCollectionConfig, collectMoment, completeFileNames, failedDestinations, ct));

            if (failedDestinations.IsEmpty && completeFileNames.IsEmpty)
            {
                throw new Exception($"No files to collect for Data '{dataCollectionConfig.DataCollectionName}'");
            }

            if (failedDestinations.Any())
            {
                // clear partial collection
                await GarbageCollectFailedDestinationsAsync(failedDestinations, completeFileNames, dataCollectionConfig, cancellationToken);
            }
        }

        private async Task CollectRouteAsync((string CollectUrl, IEnumerable<IDestination> Destinations) route, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, 
            ConcurrentBag<(IDestination, string)> completeFileNames, ConcurrentBag<IDestination> failedDestinations, CancellationToken cancellationToken)
        {
            foreach (var destination in route.Destinations)
            {
                try
                {
                    await CollectToDestinationAsync(route.CollectUrl, destination, dataCollectionConfig, collectMoment, completeFileNames, cancellationToken);
                }
                catch
                {
                    failedDestinations.Add(destination);
                    // do not throw exception, give other destinations a chance
                }
            }
        }

        private async Task CollectToDestinationAsync(string collectUrl, IDestination destination, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, ConcurrentBag<(IDestination, string)> completeFileNames, CancellationToken cancellationToken)
        {
            var destinationFileName =
                  _fileNameProvider.GenerateCollectDestinationFileName(
                      dataCollectionConfig.DataCollectionName,
                      collectUrl,
                      collectMoment,
                      !string.IsNullOrEmpty(dataCollectionConfig.CollectFileIdentifiersUrl)
                    );

            var tryNo = 1;
            await Policy
                .Handle<Exception>(e => IsRetryableCollectError(e, dataCollectionConfig))
                .WaitAndRetryAsync(
                    2,
                    (x) => TimeSpan.FromSeconds(5),
                    (exception, timeSpan) =>
                    {
                        tryNo++;   
                    }
                )
                .ExecuteAsync((ct) =>
                    {
                        return destination.CollectAsync(collectUrl, dataCollectionConfig.CollectHeaders, dataCollectionConfig.DataCollectionName, destinationFileName, dataCollectionConfig.CollectTimeout, dataCollectionConfig.CollectFinishWait, tryNo, ct);
                    },
                    cancellationToken
                );

            completeFileNames.Add((destination, destinationFileName));
        }

        private bool IsRetryableCollectError(Exception e, DataCollectionConfig dataCollectionConfig)
        {
            string[] RetryableErrorMessages = { "Gateway Timeout 504", "The request timed out" };

            return RetryableErrorMessages.Any(x => e.Message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task GarbageCollectFailedDestinationsAsync(IEnumerable<IDestination> failedDestinations, IEnumerable<(IDestination Destination, string FileName)> completeFileNames, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            var failures =
                    failedDestinations
                    .Distinct()
                    .Select(x => (Destination: x, CompleteFileNames: completeFileNames.Where(y => y.Destination == x).Select(y => y.FileName)));

            await ParallelUtils.ParallelEnumerateAsync(failures, cancellationToken, (dataCollectionConfig.ParallelDestinations ? 2 : 1),
                (failure, ct) => GarbageCollectDestinationFilesAsync(failure.Destination, dataCollectionConfig.DataCollectionName, failure.CompleteFileNames, ct));
        }

        public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            var destinations = await GetDestinationsAsync(dataCollectionConfig.DestinationIds, cancellationToken);
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
