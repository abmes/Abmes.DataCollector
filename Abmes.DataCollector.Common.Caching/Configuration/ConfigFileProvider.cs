using Abmes.DataCollector.Common.Caching.Cache;
using Abmes.DataCollector.Common.Configuration;

namespace Abmes.DataCollector.Common.Caching.Configuration;

public class ConfigFileProvider(
    IConfigProvider configFileProvider,
    IConfigFileCache configFileCache) : IConfigProvider
{
    public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
    {
        return await configFileCache.GetConfigFileContentAsync(fileName, configFileProvider, cancellationToken);
    }
}
