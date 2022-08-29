namespace Abmes.DataCollector.Vault.Configuration;

public interface IStoragesConfigProvider
{
    Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken);
}
