using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage;

public interface IStorageResolverProvider
{
    IStorageResolver GetResolver(StorageConfig storageConfig);
}
