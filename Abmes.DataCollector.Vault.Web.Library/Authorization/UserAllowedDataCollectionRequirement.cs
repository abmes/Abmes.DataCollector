using Microsoft.AspNetCore.Authorization;

namespace Abmes.DataCollector.Vault.Web.Library.Authorization;

public class UserAllowedDataCollectionRequirement : IAuthorizationRequirement
{
    public UserAllowedDataCollectionRequirement()
    {
    }
}