using Abmes.DataCollector.Collector.Data.Web.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.Web;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<WebDestination>().As<IWebDestination>();
        builder.RegisterType<WebDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
