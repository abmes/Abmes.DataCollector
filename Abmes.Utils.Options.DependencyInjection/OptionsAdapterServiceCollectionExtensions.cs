using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsAdapter<TOptions>(this IServiceCollection services) where TOptions : class, new()
    {
        return services.AddOptionsAdapter<TOptions, TOptions>();
    }

    public static IServiceCollection AddOptionsAdapter<TTo, TFrom>(this IServiceCollection services) where TTo : class where TFrom : class, TTo, new()
    {
        services.AddOptions();
        return services.AddAdapter<TTo, IOptions<TFrom>>(o => o.Value);
    }
}
