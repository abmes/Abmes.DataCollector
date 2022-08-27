namespace Abmes.DataCollector.Vault.Configuration;

public interface IUsersProvider
{
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);
}
