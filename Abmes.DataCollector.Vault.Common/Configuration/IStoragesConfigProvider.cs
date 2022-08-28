namespace Abmes.DataCollector.Vault.Configuration;

public interface IStoragesConfigProvider
{
    Task<IEnumerable<IStorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken);
}
