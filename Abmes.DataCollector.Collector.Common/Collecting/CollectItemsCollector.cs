using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Utils;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

        public async Task<IEnumerable<string>> CollectItemsAsync(IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken)
        {
            var routes = collectItems.Select(x => (CollectItem: x, Targets: destinations.Select(y => (Destination: y, DestinationFileName: GetDestinationFileName(x, dataCollectionConfig, y, collectMoment)))));

            var lockTargets = GetLockTargets(routes.SelectMany(x => x.Targets));

            if (dataCollectionConfig.ParallelDestinationCount > 1)
            {
                routes = routes.SelectMany(x => x.Targets.Select(y => (CollectItem: x.CollectItem, Targets: (IEnumerable<(IDestination Destination, string DestinationFileName)>)(new[] { (y.Destination, y.DestinationFileName) }))));
            }

            var completeFileNames = new ConcurrentBag<(IDestination Destination, string FileName)>();
            var failedDestinations = new ConcurrentBag<IDestination>();

            await CollectRouteAsync(default, lockTargets, dataCollectionConfig, collectMoment, completeFileNames, failedDestinations, cancellationToken);

            await ParallelUtils.ParallelEnumerateAsync(routes, cancellationToken, Math.Max(1, dataCollectionConfig.ParallelDestinationCount ?? 0),
                (route, ct) => CollectRouteAsync(route.CollectItem, route.Targets, dataCollectionConfig, collectMoment, completeFileNames, failedDestinations, ct));

            if (failedDestinations.Any())
            {
                await GarbageCollectFailedDestinationsAsync(failedDestinations, completeFileNames, dataCollectionConfig, cancellationToken);

                var failedDestinationNames = string.Join(", ", failedDestinations.Select(x => x.DestinationConfig.DestinationId));

                throw new Exception($"Failed to collect data to destinations '{failedDestinationNames}'");
            }

            await GarbageCollectTargetsAsync(lockTargets.GroupBy(x => x.Destination).Select(x => (Destination: x.Key, DestinationFileNames: x.Select(y => y.DestinationFileName))), dataCollectionConfig, cancellationToken);

            return completeFileNames.Select(x => x.FileName).Distinct();
        }

        private string GetDestinationFileName((IFileInfo CollectFileInfo, string CollectUrl) collectItem, DataCollectionConfig dataCollectionConfig, IDestination destination, DateTimeOffset collectMoment)
        {
            return
                _fileNameProvider.GenerateCollectDestinationFileName(
                    dataCollectionConfig.DataCollectionName,
                    collectItem.CollectFileInfo?.Name,
                    collectItem.CollectUrl,
                    collectMoment,
                    destination.DestinationConfig.CollectToDirectories,
                    destination.DestinationConfig.GenerateFileNames
                );
        }

        private IEnumerable<(IDestination Destination, string DestinationFileName)> GetLockTargets(IEnumerable<(IDestination Destination, string DestinationFileName)> targets)
        {
            return
                targets
                .Select(x => (x.Destination, DestinationDirName: string.Join("/", x.DestinationFileName.Split("/").SkipLast(1))))
                .GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => (x.Key.Destination, DestinationFileName: x.Key.DestinationDirName + "/" + _fileNameProvider.LockFileName));
        }

        private async Task CollectRouteAsync((IFileInfo CollectFileInfo, string CollectUrl) collectItem, IEnumerable<(IDestination Destination, string DestinationFileName)> targets, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment,
            ConcurrentBag<(IDestination, string)> completeFileNames, ConcurrentBag<IDestination> failedDestinations, CancellationToken cancellationToken)
        {
            foreach (var target in targets)
            {
                try
                {
                    await CollectToDestinationAsync(collectItem, target.Destination, target.DestinationFileName, dataCollectionConfig, collectMoment, completeFileNames, cancellationToken);
                }
                catch
                {
                    failedDestinations.Add(target.Destination);
                    // do not throw exception, give other destinations a chance
                }
            }
        }

        private async Task CollectToDestinationAsync((IFileInfo CollectFileInfo, string CollectUrl) collectItem, IDestination destination, string destinationFileName, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, ConcurrentBag<(IDestination, string)> completeFileNames, CancellationToken cancellationToken)
        {
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
                .ExecuteAsync(async (ct) =>
                {
                    if (string.IsNullOrEmpty(collectItem.CollectUrl))
                    {
                        await destination.PutFileAsync(dataCollectionConfig.DataCollectionName, destinationFileName, new MemoryStream(), cancellationToken);
                    }
                    else
                    {
                        await destination.CollectAsync(collectItem.CollectUrl, dataCollectionConfig.CollectHeaders, dataCollectionConfig.IdentityServiceClientInfo, dataCollectionConfig.DataCollectionName, destinationFileName, dataCollectionConfig.CollectTimeout ?? default, dataCollectionConfig.CollectFinishWait ?? false, tryNo, ct);
                    }
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
            var failedTargets =
                    failedDestinations
                    .Distinct()
                    .Select(x => (Destination: x, DestinationFileNames: completeFileNames.Where(y => y.Destination == x).Select(y => y.FileName)));

            await GarbageCollectTargetsAsync(failedTargets, dataCollectionConfig, cancellationToken);
        }

        private async Task GarbageCollectTargetsAsync(IEnumerable<(IDestination Destination, IEnumerable<string> DestinationFileNames)> targets, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            await ParallelUtils.ParallelEnumerateAsync(targets, cancellationToken, Math.Max(1, dataCollectionConfig.ParallelDestinationCount ?? 1),
                (target, ct) => GarbageCollectDestinationFilesAsync(target.Destination, dataCollectionConfig.DataCollectionName, target.DestinationFileNames, ct));
        }

        private async Task GarbageCollectDestinationFilesAsync(IDestination destination, string dataCollectionName, IEnumerable<string> fileNames, CancellationToken cancellationToken)
        {
            await Task.WhenAll(fileNames.Select(x => destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, x, cancellationToken)));
        }
    }
}
