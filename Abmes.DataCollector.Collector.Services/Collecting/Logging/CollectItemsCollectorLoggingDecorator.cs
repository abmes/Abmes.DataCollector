using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Services.Collecting.Logging;

public class CollectItemsCollectorLoggingDecorator(
    ILogger<CollectItemsCollectorLoggingDecorator> logger,
    ICollectItemsCollector collectItemsCollector)
    : ICollectItemsCollector
{
    public async Task<IEnumerable<string>> CollectItemsAsync(
        IEnumerable<CollectItem> collectItems,
        string dataCollectionName,
        IEnumerable<IDestination> destinations,
        DataCollectionConfig dataCollectionConfig,
        DateTimeOffset collectMoment,
        CancellationToken cancellationToken)
    {
        collectItems = collectItems.ToList().AsEnumerable();  // enumerate

        logger.LogInformation("Found {count} items to collect for data collection '{dataCollectionName}'", collectItems.Count(), dataCollectionName);

        return await collectItemsCollector.CollectItemsAsync(
            collectItems, dataCollectionName, destinations, dataCollectionConfig, collectMoment, cancellationToken);
    }
}
