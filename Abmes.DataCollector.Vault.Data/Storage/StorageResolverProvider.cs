using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage;

public class StorageResolverProvider(
    IEnumerable<IStorageResolver> storageResolvers) : IStorageResolverProvider
{
    public IStorageResolver GetResolver(StorageConfig storageConfig)
    {
        return storageResolvers.Where(x => x.CanResolve(storageConfig)).Single();
    }
}
