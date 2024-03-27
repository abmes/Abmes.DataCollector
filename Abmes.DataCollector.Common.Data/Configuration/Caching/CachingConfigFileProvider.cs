namespace Abmes.DataCollector.Common.Data.Configuration.Caching;

public class CachingConfigFileProvider(
    IConfigProvider configFileProvider,
    IConfigFileCache configFileCache) : IConfigProvider
{
    public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
    {
        return await configFileCache.GetConfigFileContentAsync(fileName, configFileProvider, cancellationToken);
    }
}
