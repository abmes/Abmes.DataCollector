using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Services.Configuration;

public class StoragesConfigProvider(
    IStoragesJsonConfigProvider storageJsonConfigProvider,
    IConfigProvider configProvider) : IStoragesConfigProvider
{
    private const string StorageConfigName = "StoragesConfig.json";

    public async Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken)
    {
        var json = await configProvider.GetConfigContentAsync(StorageConfigName, cancellationToken);
        var result = storageJsonConfigProvider.GetStorageConfigs(json);

        return
            result.Any(x => string.IsNullOrEmpty(x.StorageType))
            ? throw new Exception("Invalid StorageType")
            : result;
    }
}
