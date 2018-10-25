using Microsoft.AspNetCore.Authorization;

namespace Abmes.DataCollector.Vault.WebAPI.Authorization
{
    public class UserAllowedDataCollectionRequirement : IAuthorizationRequirement
    {
        public UserAllowedDataCollectionRequirement()
        {
        }
    }
}