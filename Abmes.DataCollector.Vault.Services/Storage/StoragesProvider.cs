using Abmes.DataCollector.Vault.Services.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage;

public class StoragesProvider(
    IStoragesConfigProvider storageConfigProvider,
    IStorageProvider storageProvider)
    : IStoragesProvider
{
    public async Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken)
    {
        var storageConfig = await storageConfigProvider.GetStorageConfigsAsync(cancellationToken);
        return storageConfig.Select(x => storageProvider.GetStorage(x));
    }
}
