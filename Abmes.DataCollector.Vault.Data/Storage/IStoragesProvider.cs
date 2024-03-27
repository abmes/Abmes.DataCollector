namespace Abmes.DataCollector.Vault.Data.Storage;

public interface IStoragesProvider
{
    Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken);
}
