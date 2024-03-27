namespace Abmes.DataCollector.Vault.Data.Configuration;

public interface IUsersJsonProvider
{
    IEnumerable<User> GetUsers(string json);
}
