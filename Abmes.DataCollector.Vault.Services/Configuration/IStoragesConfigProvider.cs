using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Services.Configuration;

public interface IStoragesConfigProvider
{
    Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken);
}
