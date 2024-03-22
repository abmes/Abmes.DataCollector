using Abmes.DataCollector.Common.Data.Azure.Configuration;
using Abmes.DataCollector.Common.Data.Azure.Storage;
using Abmes.DataCollector.Common.Configuration;
using Autofac;

namespace Abmes.DataCollector.Common.Data.Azure;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        builder.RegisterType<AzureCommonStorage>().As<IAzureCommonStorage>();
    }
}
