using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataCollector
    {
        Task<(IEnumerable<string> NewFileNames, IEnumerable<IFileInfo> CollectionFileInfos)> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken);
        Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, IEnumerable<IFileInfo> collectionFileInfos, CancellationToken cancellationToken);
    }
}
