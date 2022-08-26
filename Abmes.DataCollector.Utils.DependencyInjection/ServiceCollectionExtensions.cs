using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Abmes.DataCollector.Utils.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFactoryFunc<TService>(this IServiceCollection services) where TService : class
    {
        return services.AddTransient<Func<TService>>(ctx => () => ctx.GetRequiredService<TService>());
    }

    public static IServiceCollection AddAdapter<TTo, TFrom>(this IServiceCollection services, Func<TFrom, TTo> adapterFunc) where TTo : class where TFrom : class
    {
        return services.AddTransient<TTo>(serviceProvider => adapterFunc(serviceProvider.GetRequiredService<TFrom>()));
    }

    public static IServiceCollection AddOptionsAdapter<TOptions>(this IServiceCollection services) where TOptions : class, new()
    {
        return services.AddOptionsAdapter<TOptions, TOptions>();
    }

    public static IServiceCollection AddOptionsAdapter<TTo, TFrom>(this IServiceCollection services) where TTo : class where TFrom : class, TTo, new()
    {
        return services.AddAdapter<TTo, IOptions<TFrom>>(OptionsAdapter.Adapt);
    }
}
