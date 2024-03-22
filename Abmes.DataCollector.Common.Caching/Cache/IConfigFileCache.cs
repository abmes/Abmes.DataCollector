using Abmes.DataCollector.Common.Data.Configuration;

namespace Abmes.DataCollector.Common.Caching.Cache;

public interface IConfigFileCache
{
    Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
}
