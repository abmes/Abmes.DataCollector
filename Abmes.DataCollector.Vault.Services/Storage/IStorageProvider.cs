using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage;

public interface IStorageProvider
{
    IStorage GetStorage(StorageConfig storageConfig);
}
