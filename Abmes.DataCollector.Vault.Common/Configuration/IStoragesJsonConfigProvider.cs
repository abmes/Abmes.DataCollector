namespace Abmes.DataCollector.Vault.Configuration
{
    public interface IStoragesJsonConfigProvider
    {
        IEnumerable<StorageConfig> GetStorageConfigs(string json);
    }
}
