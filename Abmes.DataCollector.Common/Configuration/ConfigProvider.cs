namespace Abmes.DataCollector.Common.Configuration;

public class ConfigProvider : IConfigProvider
{
    private readonly IEnumerable<IConfigLoader> _configLoaders;
    private readonly ICommonAppSettings _commonAppSettings;
    private readonly IConfigLocationProvider _configLocationProvider;

    public ConfigProvider(
        IEnumerable<IConfigLoader> configLoaders,
        ICommonAppSettings commonAppSettings,
        IConfigLocationProvider configLocationProvider)
    {
        _configLoaders = configLoaders;
        _commonAppSettings = commonAppSettings;
        _configLocationProvider = configLocationProvider;
    }

    public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        var configLocation = _configLocationProvider.GetConfigLocation();

        return
            string.IsNullOrEmpty(configLocation) ?
            await GetConfigContentFromStorageAsync(configName, cancellationToken) :
            await GetConfigContentFromLocationAsync(configName, configLocation, cancellationToken);
    }

    private async Task<string> GetConfigContentFromLocationAsync(string configName, string configLocation, CancellationToken cancellationToken)
    {
        var configLoader = _configLoaders.Where(x => x.CanLoadFromLocation(configLocation)).FirstOrDefault();

        if (configLoader == null)
        {
            throw new Exception($"Can't provide configuration from the specified location");
        }

        return await configLoader.GetConfigContentAsync(configName, configLocation, cancellationToken);
    }

    private async Task<string> GetConfigContentFromStorageAsync(string configName, CancellationToken cancellationToken)
    { 
        if (string.IsNullOrEmpty(_commonAppSettings.ConfigStorageType))
        {
            throw new Exception("ConfigStorageType not specified!");
        }

        var configLoader = _configLoaders.Where(x => x.CanLoadFromStorage(_commonAppSettings.ConfigStorageType)).FirstOrDefault();

        if (configLoader == null)
        {
            throw new Exception($"Can't provide configuration from storage type '{_commonAppSettings.ConfigStorageType}'");
        }

        return await configLoader.GetConfigContentAsync(configName, cancellationToken);
    }
}
