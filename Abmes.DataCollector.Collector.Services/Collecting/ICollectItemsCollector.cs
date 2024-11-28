using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public interface ICollectItemsCollector
{
    Task<IEnumerable<string>> CollectItemsAsync(IEnumerable<CollectItem> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken);
}
