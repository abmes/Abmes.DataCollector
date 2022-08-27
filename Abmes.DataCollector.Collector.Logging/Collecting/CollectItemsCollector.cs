using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Storage;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class CollectItemsCollector : ICollectItemsCollector
    {
        private readonly ILogger<CollectItemsCollector> _logger;
        private readonly ICollectItemsCollector _collectItemsCollector;

        public CollectItemsCollector(
            ILogger<CollectItemsCollector> logger,
            ICollectItemsCollector collectItemsCollector)
        {
            _logger = logger;
            _collectItemsCollector = collectItemsCollector;
        }

        public async Task<IEnumerable<string>> CollectItemsAsync(IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken)
        {
            collectItems = collectItems.ToList().AsEnumerable();  // enumerate

            _logger.LogInformation($"Found {collectItems.Count()} items to collect for data collection '{dataCollectionName}'");

            return await _collectItemsCollector.CollectItemsAsync(collectItems, dataCollectionName, destinations, dataCollectionConfig, collectMoment, cancellationToken);
        }
    }
}
