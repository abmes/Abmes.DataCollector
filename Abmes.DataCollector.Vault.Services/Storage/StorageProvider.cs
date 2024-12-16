using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage;

public class StorageProvider(
    IStorageResolverProvider storageResolverProvider)
    : IStorageProvider
{
    public IStorage GetStorage(StorageConfig storageConfig)
    {
        var resolver = storageResolverProvider.GetResolver(storageConfig);
        return resolver.GetStorage(storageConfig);
    }
}
