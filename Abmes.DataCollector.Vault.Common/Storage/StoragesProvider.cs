using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public class StoragesProvider : IStoragesProvider
{
    private readonly IStoragesConfigProvider _storageConfigProvider;
    private readonly IStorageProvider _storageProvider;

    public StoragesProvider(
        IStoragesConfigProvider storageConfigProvider,
        IStorageProvider storageProvider)
    {
        _storageConfigProvider = storageConfigProvider;
        _storageProvider = storageProvider;
    }

    public async Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken)
    {
        var storageConfig = await _storageConfigProvider.GetStorageConfigsAsync(cancellationToken);
        return storageConfig.Select(x => _storageProvider.GetStorage(x));
    }
}
