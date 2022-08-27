using Abmes.DataCollector.Common.Caching.Cache;
using Abmes.DataCollector.Common.Configuration;

namespace Abmes.DataCollector.Common.Caching.Configuration;

public class ConfigFileProvider : IConfigProvider
{
    private readonly IConfigProvider _configFileProvider;
    private readonly IConfigFileCache _configFileCache;

    public ConfigFileProvider(
        IConfigProvider configFileProvider,
        IConfigFileCache configFileCache)
    {
        _configFileProvider = configFileProvider;
        _configFileCache = configFileCache;
    }

    public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
    {
        return await _configFileCache.GetConfigFileContentAsync(fileName, _configFileProvider, cancellationToken);
    }
}
