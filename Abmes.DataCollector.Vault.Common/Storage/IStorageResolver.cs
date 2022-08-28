using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public interface IStorageResolver
{
    IStorage GetStorage(IStorageConfig storageConfig);
    bool CanResolve(IStorageConfig storageConfig);
}
