using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Data.Storage;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public interface IDataCollector
{
    Task<(IEnumerable<string> NewFileNames, IEnumerable<FileInfoData> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken);
    Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<FileInfoData> collectionFileInfos, CancellationToken cancellationToken);
}
