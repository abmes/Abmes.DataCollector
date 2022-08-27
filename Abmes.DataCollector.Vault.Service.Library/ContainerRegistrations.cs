using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Vault.Service;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Common.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.FileSystem.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Amazon.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.FileSystem.ContainerRegistrations.RegisterFor(builder);

        builder.RegisterType<Abmes.DataCollector.Vault.WebAPI.Configuration.DataCollectionNameProvider>().Named<IDataCollectionNameProvider>("base");

        Abmes.DataCollector.Common.Logging.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Logging.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Common.Caching.ContainerRegistrations.RegisterFor(builder);
    }
}
