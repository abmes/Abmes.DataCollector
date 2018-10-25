using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Configuration
{
    public interface IDataFilterProvider
    {
        Task<string> GetDataFilterAsync(CancellationToken cancellationToken);
    }
}
