using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Azure.Storage;

public class AzureStorageResolver(
    IAzureStorageFactory AzureStorageFactory) : IStorageResolver
{
    public bool CanResolve(StorageConfig storageConfig)
    {
        return string.Equals(storageConfig.StorageType, "Azure");
    }

    public IStorage GetStorage(StorageConfig storageConfig)
    {
        return AzureStorageFactory(storageConfig);
    }
}
