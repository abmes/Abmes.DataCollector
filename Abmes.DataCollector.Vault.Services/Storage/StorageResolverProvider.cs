using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage;

public class StorageResolverProvider(
    IEnumerable<IStorageResolver> storageResolvers) : IStorageResolverProvider
{
    public IStorageResolver GetResolver(StorageConfig storageConfig)
    {
        return storageResolvers.Where(x => x.CanResolve(storageConfig)).Single();
    }
}
