namespace Abmes.DataCollector.Vault.Services.Users;

public interface IUsersJsonProvider
{
    IEnumerable<User> GetUsers(string json);
}
