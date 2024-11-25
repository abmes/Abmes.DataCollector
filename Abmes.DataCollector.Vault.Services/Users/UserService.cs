using Abmes.DataCollector.Vault.Services.Contracts;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Users;

namespace Abmes.DataCollector.Vault.Services.Users;

public class UserService(
    IUsersProvider usersProvider,
    IUserExternalIdentifierProvider userExternalIdentifierProvider,
    IDataCollectionNameProvider dataCollectionNameProvider)
    : IUserService
{
    public async Task<bool> IsUserAllowedDataCollectionAsync(CancellationToken cancellationToken)
    {
        var dataCollectionName = dataCollectionNameProvider.GetDataCollectionName();
        var identityUserId = await userExternalIdentifierProvider.GetUserExternalIdentifierAsync(cancellationToken);
        var users = await usersProvider.GetUsersAsync(cancellationToken);

        return
            users
            .Where(x => x.IdentityUserId == identityUserId)
            .Where(x => x.DataCollectionNames.Contains(dataCollectionName))
            .Any();
    }
}
