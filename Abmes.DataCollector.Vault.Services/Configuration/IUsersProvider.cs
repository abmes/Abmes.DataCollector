namespace Abmes.DataCollector.Vault.Services.Configuration;

public interface IUsersProvider
{
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);
}
