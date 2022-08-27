using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public interface IStorageResolver
{
    IStorage GetStorage(StorageConfig storageConfig);
    bool CanResolve(StorageConfig storageConfig);
}
