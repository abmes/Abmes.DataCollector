namespace Abmes.DataCollector.Vault.Data.Configuration;

public interface IStoragesConfigProvider
{
    Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken);
}
