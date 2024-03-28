namespace Abmes.DataCollector.Vault.Services.Configuration;

public interface IUsersJsonProvider
{
    IEnumerable<User> GetUsers(string json);
}
