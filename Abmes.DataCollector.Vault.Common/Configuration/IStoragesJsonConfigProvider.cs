namespace Abmes.DataCollector.Vault.Configuration;

public interface IStoragesJsonConfigProvider
{
    IEnumerable<IStorageConfig> GetStorageConfigs(string json);
}
