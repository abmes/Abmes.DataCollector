namespace Abmes.DataCollector.Vault.Storage;

public interface IStoragesProvider
{
    Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken);
}
