using Abmes.DataCollector.Shared.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Services.Users;

public class UsersProvider(
    IUsersJsonProvider usersJsonProvider,
    IConfigProvider configProvider) : IUsersProvider
{
    private const string UsersConfigName = "Users.json";

    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var json = await configProvider.GetConfigContentAsync(UsersConfigName, cancellationToken);
        return usersJsonProvider.GetUsers(json);
    }
}
