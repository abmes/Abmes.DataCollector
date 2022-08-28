using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Azure.Storage;

public class AzureStorageResolver : IStorageResolver
{
    private readonly IAzureStorageFactory _AzureStorageFactory;

    public AzureStorageResolver(
        IAzureStorageFactory AzureStorageFactory)
    {
        _AzureStorageFactory = AzureStorageFactory;
    }

    public bool CanResolve(IStorageConfig storageConfig)
    {
        return string.Equals(storageConfig.StorageType, "Azure");
    }

    public IStorage GetStorage(IStorageConfig storageConfig)
    {
        return _AzureStorageFactory(storageConfig);
    }
}
