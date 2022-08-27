namespace Abmes.DataCollector.Vault.Configuration;

public interface IUsersJsonProvider
{
    IEnumerable<User> GetUsers(string json);
}
