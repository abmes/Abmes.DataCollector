using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class CollectItemsCollector : ICollectItemsCollector
    {
        private readonly IFileNameProvider _fileNameProvider;

        public CollectItemsCollector(
            IFileNameProvider fileNameProvider)
        {
            _fileNameProvider = fileNameProvider;
        }

        public async Task CollectItemsAsync(IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken)
        {
            var routes = collectItems.Select(x => (CollectItem: x, Destinations: destinations));

            if (dataCollectionConfig.ParallelDestinationCount > 1)
            {
                routes = routes.SelectMany(x => x.Destinations.Select(y => (CollectItem: x.CollectItem, Destinations: (IEnumerable<IDestination>)(new[] { y }))));
            }

            var completeFileNames = new ConcurrentBag<(IDestination Destination, string FileName)>();
            var failedDestinations = new ConcurrentBag<IDestination>();

            await ParallelUtils.ParallelEnumerateAsync(routes, cancellationToken, Math.Max(1, dataCollectionConfig.ParallelDestinationCount),
                (route, ct) => CollectRouteAsync(route, dataCollectionConfig, collectMoment, completeFileNames, failedDestinations, ct));

            if (failedDestinations.Any())
            {
                await GarbageCollectFailedDestinationsAsync(failedDestinations, completeFileNames, dataCollectionConfig, cancellationToken);

                var failedDestinationNames = string.Join(", ", failedDestinations.Select(x => x.DestinationConfig.DestinationId));

                throw new Exception($"Failed to collect data to destinations '{failedDestinationNames}'");
            }
        }

        private async Task CollectRouteAsync(((IFileInfo CollectFileInfo, string CollectUrl) CollectItem, IEnumerable<IDestination> Destinations) route, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment,
            ConcurrentBag<(IDestination, string)> completeFileNames, ConcurrentBag<IDestination> failedDestinations, CancellationToken cancellationToken)
        {
            foreach (var destination in route.Destinations)
            {
                try
                {
                    await CollectToDestinationAsync(route.CollectItem, destination, dataCollectionConfig, collectMoment, completeFileNames, cancellationToken);
                }
                catch
                {
                    failedDestinations.Add(destination);
                    // do not throw exception, give other destinations a chance
                }
            }
        }

        private async Task CollectToDestinationAsync((IFileInfo CollectFileInfo, string CollectUrl) collectItem, IDestination destination, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, ConcurrentBag<(IDestination, string)> completeFileNames, CancellationToken cancellationToken)
        {
            var destinationFileName =
                  _fileNameProvider.GenerateCollectDestinationFileName(
                      dataCollectionConfig.DataCollectionName,
                      collectItem.CollectUrl,
                      collectMoment,
                      destination.DestinationConfig.CollectToDirectories,
                      destination.DestinationConfig.GenerateFileNames
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
                    return destination.CollectAsync(collectItem.CollectUrl, dataCollectionConfig.CollectHeaders, dataCollectionConfig.IdentityServiceClientInfo, dataCollectionConfig.DataCollectionName, destinationFileName, dataCollectionConfig.CollectTimeout, dataCollectionConfig.CollectFinishWait, tryNo, ct);
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

            await ParallelUtils.ParallelEnumerateAsync(failures, cancellationToken, Math.Max(1, dataCollectionConfig.ParallelDestinationCount),
                (failure, ct) => GarbageCollectDestinationFilesAsync(failure.Destination, dataCollectionConfig.DataCollectionName, failure.CompleteFileNames, ct));
        }

        private async Task GarbageCollectDestinationFilesAsync(IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
        {
            await Task.WhenAll(fileNames.Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
        }
    }
}
