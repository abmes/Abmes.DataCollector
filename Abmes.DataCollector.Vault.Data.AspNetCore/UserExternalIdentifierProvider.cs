using Abmes.DataCollector.Vault.Services.Ports.Users;
using Microsoft.AspNetCore.Http;

namespace Abmes.DataCollector.Vault.Data.AspNetCore;

public class UserExternalIdentifierProvider(
    IHttpContextAccessor httpContextAccessor)
    : IUserExternalIdentifierProvider
{
    public async Task<string> GetUserExternalIdentifierAsync(CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var result = httpContext.User.FindFirst("sub")?.Value;
        ArgumentException.ThrowIfNullOrEmpty(result);

        return await Task.FromResult(result);
    }
}
