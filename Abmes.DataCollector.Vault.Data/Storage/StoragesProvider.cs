using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage;

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
