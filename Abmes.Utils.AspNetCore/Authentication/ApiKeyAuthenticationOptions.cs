using Microsoft.AspNetCore.Authentication;

namespace Abmes.Utils.AspNetCore.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string? ApiKey { get; set; }
}
