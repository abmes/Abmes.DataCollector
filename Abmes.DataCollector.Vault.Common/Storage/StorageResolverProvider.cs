using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public class StorageResolverProvider : IStorageResolverProvider
{
    private readonly IEnumerable<IStorageResolver> _storageResolvers;

    public StorageResolverProvider(IEnumerable<IStorageResolver> storageResolvers)
    {
        _storageResolvers = storageResolvers;
    }

    public IStorageResolver GetResolver(StorageConfig storageConfig)
    {
        return _storageResolvers.Where(x => x.CanResolve(storageConfig)).Single();
    }
}
