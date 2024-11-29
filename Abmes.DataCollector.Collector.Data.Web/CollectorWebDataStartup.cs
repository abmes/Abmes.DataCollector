using Abmes.DataCollector.Collector.Data.Web.Configuration;
using Abmes.DataCollector.Collector.Data.Web.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.Data.Web;

public static class CollectorWebDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();

        builder.RegisterType<WebDestination>().As<IWebDestination>();
        builder.RegisterType<WebDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
