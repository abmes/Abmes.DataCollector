namespace Abmes.DataCollector.Vault.Services.Users;

public interface IUsersProvider
{
    Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);
}
