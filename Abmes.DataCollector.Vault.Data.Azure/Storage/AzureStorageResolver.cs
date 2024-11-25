using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Data.Azure.Storage;

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
