using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public class StorageProvider(
    IStorageResolverProvider storageResolverProvider) : IStorageProvider
{
    public IStorage GetStorage(StorageConfig storageConfig)
    {
        var resolver = storageResolverProvider.GetResolver(storageConfig);
        return resolver.GetStorage(storageConfig);
    }
}
