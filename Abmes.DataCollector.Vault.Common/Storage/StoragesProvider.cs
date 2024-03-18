using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public class StoragesProvider(
    IStoragesConfigProvider storageConfigProvider,
    IStorageProvider storageProvider) : IStoragesProvider
{
    public async Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken)
    {
        var storageConfig = await storageConfigProvider.GetStorageConfigsAsync(cancellationToken);
        return storageConfig.Select(x => storageProvider.GetStorage(x));
    }
}
