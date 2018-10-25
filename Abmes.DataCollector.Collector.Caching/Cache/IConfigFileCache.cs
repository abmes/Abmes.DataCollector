using Abmes.DataCollector.Collector.Common.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Caching.Cache
{
    public interface IConfigFileCache
    {
        Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
    }
}
