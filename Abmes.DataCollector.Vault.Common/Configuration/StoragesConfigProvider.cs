using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Common.Configuration;

public class StoragesConfigProvider(
    IStoragesJsonConfigProvider storageJsonConfigProvider,
    IConfigProvider configProvider) : IStoragesConfigProvider
{
    private const string StorageConfigName = "StoragesConfig.json";

    public async Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken)
    {
        var json = await configProvider.GetConfigContentAsync(StorageConfigName, cancellationToken);
        var result = storageJsonConfigProvider.GetStorageConfigs(json);

        if (result.Any(x => string.IsNullOrEmpty(x.StorageType)))
        {
            throw new Exception("Invalid StorageType");
        }

        return result;
    }
}
