using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataCollector
    {
        Task<(IEnumerable<string> NewFileNames, IEnumerable<IFileInfoData> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken);
        Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<IFileInfoData> collectionFileInfos, CancellationToken cancellationToken);
    }
}
