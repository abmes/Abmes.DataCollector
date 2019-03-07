using Abmes.DataCollector.Collector.ConsoleApp.Configuration;
using Abmes.DataCollector.Collector.ConsoleApp.Initialization;
using Autofac;
using Abmes.DataCollector.ConsoleApp;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            Abmes.DataCollector.Common.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Common.Amazon.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Common.Azure.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Amazon.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Azure.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Web.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Console.ContainerRegistrations.RegisterFor(builder);

            Abmes.DataCollector.Collector.Common.ContainerRegistrations.RegisterFor(builder);

            builder.RegisterType<MainService>().As<IMainService>();
            builder.RegisterType<ConfigSetNameProvider>().Named<IConfigSetNameProvider>("base");
            builder.RegisterType<DataCollectionsFilterProvider>().As<IDataCollectionsFilterProvider>();

            Abmes.DataCollector.Common.Logging.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Logging.ContainerRegistrations.RegisterFor(builder);

            Abmes.DataCollector.Common.Caching.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Collector.Caching.ContainerRegistrations.RegisterFor(builder);
        }
    }
}
