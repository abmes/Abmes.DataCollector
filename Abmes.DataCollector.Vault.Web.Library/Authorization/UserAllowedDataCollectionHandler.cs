using Abmes.DataCollector.Vault.Common.Configuration;
using Abmes.DataCollector.Vault.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Abmes.DataCollector.Vault.Web.Library.Authorization;

public class UserAllowedDataCollectionHandler(
    IUsersProvider usersProvider,
    IDataCollectionNameProvider dataCollectionNameProvider) : AuthorizationHandler<UserAllowedDataCollectionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAllowedDataCollectionRequirement requirement)
    {
        var dataCollectionName = dataCollectionNameProvider.GetDataCollectionName();

        var identityUserId = context.User.Claims.Where(x => x.Type.Equals("sub")).Select(x => x.Value).FirstOrDefault();

        if (string.IsNullOrEmpty(identityUserId))
        {
            context.Fail();
        }
        else
        {
            var users = await usersProvider.GetUsersAsync(CancellationToken.None);

            var user = users.Where(x => x.IdentityUserId == identityUserId).FirstOrDefault();

            if ((user is null) || (!user.DataCollectionNames.Contains(dataCollectionName)))
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }
        }
    }
}