using Abmes.DataCollector.Common.Services.Ports.Configuration;

namespace Abmes.DataCollector.Common.Services.Configuration.Caching;

public class ConfigFileProviderCachingDecorator(
    IConfigProvider configFileProvider,
    IConfigFileCache configFileCache) : IConfigProvider
{
    public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
    {
        return await configFileCache.GetConfigFileContentAsync(fileName, configFileProvider, cancellationToken);
    }
}
