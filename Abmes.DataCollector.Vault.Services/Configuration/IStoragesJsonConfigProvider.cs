using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Services.Configuration;

public interface IStoragesJsonConfigProvider
{
    IEnumerable<StorageConfig> GetStorageConfigs(string json);
}
