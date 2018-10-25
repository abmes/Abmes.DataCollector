using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Utils.DependancyInjection
{
    public static class ServiceCollectionExtensions
    {
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
}
