namespace Microsoft.Extensions.DependencyInjection;

public static class AdapterServiceCollectionExtensions
{
    public static IServiceCollection AddFactoryFunc<TService>(this IServiceCollection services) where TService : class
    {
        return services.AddTransient<Func<TService>>(sp => () => sp.GetRequiredService<TService>());
    }

    public static IServiceCollection AddAdapter<TTo, TFrom>(this IServiceCollection services, Func<TFrom, TTo> adapterFunc) where TTo : class where TFrom : class
    {
        return services.AddTransient<TTo>(sp => adapterFunc(sp.GetRequiredService<TFrom>()));
    }
}
