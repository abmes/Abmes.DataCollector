using Abmes.DataCollector.Common.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Caching.Cache
{
    public interface IConfigFileCache
    {
        Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
    }
}
