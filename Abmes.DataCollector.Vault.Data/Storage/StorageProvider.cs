using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage;

public class StorageProvider(
    IStorageResolverProvider storageResolverProvider) : IStorageProvider
{
    public IStorage GetStorage(StorageConfig storageConfig)
    {
        var resolver = storageResolverProvider.GetResolver(storageConfig);
        return resolver.GetStorage(storageConfig);
    }
}
