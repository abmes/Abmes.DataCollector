using Autofac;
using Abmes.DataCollector.Collector.Common.Configuration;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.Service
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
        {
            Abmes.DataCollector.Common.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Common.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
            Abmes.DataCollector.Common.Azure.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
            Abmes.DataCollector.Collector.Azure.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Web.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Console.ContainerRegistrations.RegisterFor(builder);

            Abmes.DataCollector.Collector.Common.ContainerRegistrations.RegisterFor(builder);

            Abmes.DataCollector.Common.Logging.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Logging.ContainerRegistrations.RegisterFor(builder);

            Abmes.DataCollector.Common.Caching.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Caching.ContainerRegistrations.RegisterFor(builder);
        }
    }
}
