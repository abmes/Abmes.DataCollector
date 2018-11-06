using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public interface IDataCollectionsFilterProvider
    {
        Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken);
    }
}
