using Abmes.DataCollector.Common.Configuration;

namespace Abmes.DataCollector.Common.Caching.Cache
{
    public interface IConfigFileCache
    {
        Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
    }
}
