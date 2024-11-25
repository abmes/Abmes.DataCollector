using Microsoft.AspNetCore.Authorization;

namespace Abmes.DataCollector.Vault.Web.AspNetCore.Authorization;

public record UserAllowedDataCollectionRequirement : IAuthorizationRequirement;
