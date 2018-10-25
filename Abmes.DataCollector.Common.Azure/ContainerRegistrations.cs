using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Azure.Configuration;
using Abmes.DataCollector.Common.Azure.Storage;
using Abmes.DataCollector.Common.Storage;
using Autofac;

namespace Abmes.DataCollector.Common.Azure
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");
            builder.RegisterType<AzureCommonStorage>().As<IAzureCommonStorage>();
        }
    }
}
