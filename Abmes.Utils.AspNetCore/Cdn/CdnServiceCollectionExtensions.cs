using Abmes.Utils.AspNetCore.Cdn;

namespace Microsoft.Extensions.DependencyInjection;

public static class CdnServiceCollectionExtensions
{
    public static IServiceCollection AddCdnizer(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICdnizer<>), typeof(Cdnizer<>));
        return services;
    }
}
