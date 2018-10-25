using Abmes.DataCollector.Collector.Common.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataPreparer
    {
        Task PrepareDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken);
    }
}
