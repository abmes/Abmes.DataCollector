using Abmes.DataCollector.Collector.Web.Destinations;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Web
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<WebDestination>().As<IWebDestination>();
            builder.RegisterType<WebDestinationResolver>().Named<IDestinationResolver>("base");
        }
    }
}
