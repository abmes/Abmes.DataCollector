using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Amazon.Storage;

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
