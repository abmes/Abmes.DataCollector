using Abmes.DataCollector.Collector.Common.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataCollector
    {
        Task CollectDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken);
        Task GarbageCollectDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken);
    }
}
