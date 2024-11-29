using Abmes.DataCollector.Shared.Data.Azure.Configuration;
using Abmes.DataCollector.Shared.Data.Azure.Storage;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;

namespace Abmes.DataCollector.Shared.Data.Azure;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        builder.RegisterType<AzureCommonStorage>().As<IAzureCommonStorage>();
    }
}
