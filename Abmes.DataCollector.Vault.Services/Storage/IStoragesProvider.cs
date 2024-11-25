using Abmes.DataCollector.Vault.Services.Ports.Storage;

namespace Abmes.DataCollector.Vault.Services.Storage;

public interface IStoragesProvider
{
    Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken);
}
