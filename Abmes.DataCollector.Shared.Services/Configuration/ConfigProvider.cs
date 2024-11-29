using Abmes.DataCollector.Shared.Services.Ports.Configuration;

namespace Abmes.DataCollector.Shared.Services.Configuration;

public class ConfigProvider(
    IEnumerable<IConfigLoader> configLoaders,
    ICommonAppSettings commonAppSettings,
    IConfigLocationProvider configLocationProvider)
    : IConfigProvider
{
    public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        var configLocation = configLocationProvider.GetConfigLocation();

        return
            string.IsNullOrEmpty(configLocation)
            ? await GetConfigContentFromStorageAsync(configName, cancellationToken)
            : await GetConfigContentFromLocationAsync(configName, configLocation, cancellationToken);
    }

    private async Task<string> GetConfigContentFromLocationAsync(string configName, string configLocation, CancellationToken cancellationToken)
    {
        var configLoader = configLoaders.Where(x => x.CanLoadFromLocation(configLocation)).FirstOrDefault();

        return
            configLoader is not null
            ? await configLoader.GetConfigContentAsync(configName, configLocation, cancellationToken)
            : throw new Exception($"Can't provide configuration from the specified location");
    }

    private async Task<string> GetConfigContentFromStorageAsync(string configName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(commonAppSettings.ConfigStorageType);

        var configLoader = configLoaders.Where(x => x.CanLoadFromStorage(commonAppSettings.ConfigStorageType)).FirstOrDefault();

        return
            configLoader is not null
            ? await configLoader.GetConfigContentAsync(configName, cancellationToken)
            : throw new Exception($"Can't provide configuration from storage type '{commonAppSettings.ConfigStorageType}'");
    }
}
