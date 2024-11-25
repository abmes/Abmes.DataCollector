using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Services.Ports.Storage;

public interface IStorageResolver
{
    IStorage GetStorage(StorageConfig storageConfig);
    bool CanResolve(StorageConfig storageConfig);
}
