using Microsoft.AspNetCore.Authentication;

namespace Abmes.Utils.AspNetCore.Authentication;

public static class ApiKeyAuthenticationExtensions
{
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, string apiKey)
    {
        return builder.AddApiKey(authenticationScheme, null, apiKey);
    }

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, string apiKey)
    {
        return builder.AddApiKey(authenticationScheme, displayName, options => options.ApiKey = apiKey);
    }

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        return builder.AddApiKey(authenticationScheme, null, configureOptions);
    }

    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
    }
}
