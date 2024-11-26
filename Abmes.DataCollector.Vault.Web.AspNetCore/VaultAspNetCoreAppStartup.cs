using Abmes.DataCollector.Utils.AspNetCore;
using Abmes.DataCollector.Vault.Web.AspNetCore.Authorization;
using Abmes.DataCollector.Vault.Web.AspNetCore.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Vault.Web.AspNetCore;

public static class VaultAspNetCoreAppStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(Abmes.DataCollector.Vault.Web.Controllers.DummyClass).Assembly);  // must precede AddJsonOptions

        services.AddHealthChecks();

        var identityServerAuthenticationSettings =
                configuration
                .GetSection("IdentityServerAuthenticationSettings")
                .Get<IdentityServerAuthenticationSettings>();

        ArgumentNullException.ThrowIfNull(identityServerAuthenticationSettings);

        services
            .AddAuthentication(IdentityModel.AspNetCore.OAuth2Introspection.OAuth2IntrospectionDefaults.AuthenticationScheme)
            .AddOAuth2Introspection(
                IdentityModel.AspNetCore.OAuth2Introspection.OAuth2IntrospectionDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = identityServerAuthenticationSettings.Authority;

                    // this maps to the API resource name and secret
                    options.ClientId = identityServerAuthenticationSettings.ApiName;
                    options.ClientSecret = identityServerAuthenticationSettings.ApiSecret;

                    options.DiscoveryPolicy.AuthorityValidationStrategy = new IdentityModel.Client.StringComparisonAuthorityValidationStrategy(StringComparison.OrdinalIgnoreCase);
                });

        services.AddAuthorizationBuilder()
            .AddPolicy("UserAllowedDataCollection", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new UserAllowedDataCollectionRequirement());
            });

        services.AddSingleton<IAuthorizationHandler, UserAllowedDataCollectionHandler>();
    }

    public static void Configure(IApplicationBuilder app, IEndpointRouteBuilder erb, IWebHostEnvironment env)
    {
        // order of middlewares is important
        app.UseMiddleware<BasicExceptionHandlingMiddleware>();
        app.UseMiddleware<ExceptionLoggingMiddleware>();

        // good order of standard middlewares is documented here: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        erb.MapEndpoints();
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder erb)
    {
        erb.MapControllers();
        erb.MapHealthChecks("/Health");

        return erb;
    }
}
