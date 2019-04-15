using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task CollectItemsAsync(IEnumerable<(IFileInfo CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken)
        {
            collectItems = collectItems.ToList().AsEnumerable();  // enumerate

            _logger.LogInformation($"Found {collectItems.Count()} items to collect");

            await _collectItemsCollector.CollectItemsAsync(collectItems, dataCollectionName, destinations, dataCollectionConfig, collectMoment, cancellationToken);
        }
    }
}
