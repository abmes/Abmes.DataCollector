using Abmes.DataCollector.Shared.Services.Ports.Configuration;

namespace Abmes.DataCollector.Shared.Services.Configuration.Caching;

public class ConfigFileProviderCachingDecorator(
    IConfigProvider configFileProvider,
    IConfigFileCache configFileCache)
    : IConfigProvider
{
    public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
    {
        return await configFileCache.GetConfigFileContentAsync(fileName, configFileProvider, cancellationToken);
    }
}
