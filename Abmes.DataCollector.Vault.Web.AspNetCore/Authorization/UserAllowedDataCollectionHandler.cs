using Abmes.DataCollector.Vault.Services.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Abmes.DataCollector.Vault.Web.AspNetCore.Authorization;

public class UserAllowedDataCollectionHandler(
    IUserService userService)
    : AuthorizationHandler<UserAllowedDataCollectionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAllowedDataCollectionRequirement requirement)
    {
        if (await userService.IsUserAllowedDataCollectionAsync(CancellationToken.None))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}