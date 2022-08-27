using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public interface IStorageProvider
{
    IStorage GetStorage(StorageConfig storageConfig);
}
