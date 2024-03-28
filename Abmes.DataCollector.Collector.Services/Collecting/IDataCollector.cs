using Abmes.DataCollector.Collector.Services.Abstractions;
using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Common;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public interface IDataCollector
{
    Task<(IEnumerable<string> NewFileNames, IEnumerable<FileInfoData> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken);
    Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<FileInfoData> collectionFileInfos, CancellationToken cancellationToken);
}
