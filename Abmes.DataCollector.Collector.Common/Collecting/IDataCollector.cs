using Abmes.DataCollector.Collector.Common.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataCollector
    {
        Task CollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken);
        Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken);
    }
}
