using Abmes.DataCollector.Vault.Data.Configuration;

namespace Abmes.DataCollector.Vault.Data.Storage;

public interface IStorageProvider
{
    IStorage GetStorage(StorageConfig storageConfig);
}
