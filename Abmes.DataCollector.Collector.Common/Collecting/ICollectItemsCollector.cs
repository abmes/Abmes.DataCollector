using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public interface ICollectItemsCollector
{
    Task<IEnumerable<string>> CollectItemsAsync(IEnumerable<CollectItem> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken);
}
