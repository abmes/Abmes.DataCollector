using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public interface IStorageResolverProvider
{
    IStorageResolver GetResolver(StorageConfig storageConfig);
}
