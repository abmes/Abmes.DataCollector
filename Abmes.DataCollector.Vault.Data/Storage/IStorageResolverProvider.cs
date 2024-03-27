using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage;

public interface IStorageResolverProvider
{
    IStorageResolver GetResolver(StorageConfig storageConfig);
}
