using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ICollectItemsCollector
    {
        Task<IEnumerable<string>> CollectItemsAsync(IEnumerable<(IFileInfoData CollectFileInfo, string CollectUrl)> collectItems, string dataCollectionName, IEnumerable<IDestination> destinations, DataCollectionConfig dataCollectionConfig, DateTimeOffset collectMoment, CancellationToken cancellationToken);
    }
}
