using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage;

public interface IStorageResolver
{
    IStorage GetStorage(StorageConfig storageConfig);
    bool CanResolve(StorageConfig storageConfig);
}
