using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public class StorageResolverProvider(
    IEnumerable<IStorageResolver> storageResolvers) : IStorageResolverProvider
{
    public IStorageResolver GetResolver(StorageConfig storageConfig)
    {
        return storageResolvers.Where(x => x.CanResolve(storageConfig)).Single();
    }
}
