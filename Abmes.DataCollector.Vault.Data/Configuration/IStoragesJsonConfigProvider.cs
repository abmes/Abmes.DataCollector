namespace Abmes.DataCollector.Vault.Data.Configuration;

public interface IStoragesJsonConfigProvider
{
    IEnumerable<StorageConfig> GetStorageConfigs(string json);
}
