using Abmes.DataCollector.Vault.Data.Configuration;
using Abmes.DataCollector.Vault.Data.Storage;

namespace Abmes.DataCollector.Vault.Data.Amazon.Storage;

public class AmazonStorageResolver(
    IAmazonStorageFactory amazonStorageFactory) : IStorageResolver
{
    public bool CanResolve(StorageConfig storageConfig)
    {
        return string.Equals(storageConfig.StorageType, "Amazon");
    }

    public IStorage GetStorage(StorageConfig storageConfig)
    {
        return amazonStorageFactory(storageConfig);
    }
}
