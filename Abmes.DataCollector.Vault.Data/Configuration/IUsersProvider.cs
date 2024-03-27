namespace Abmes.DataCollector.Vault.Data.Configuration;

public interface IUsersProvider
{
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);
}
