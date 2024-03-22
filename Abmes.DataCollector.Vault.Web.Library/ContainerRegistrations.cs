using Abmes.DataCollector.Vault.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Vault.Web.Library;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.Data.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Common.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Data.Amazon.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);

        builder.RegisterType<Abmes.DataCollector.Vault.Web.Library.Configuration.DataCollectionNameProvider>().Named<IDataCollectionNameProvider>("base");

        Abmes.DataCollector.Common.Logging.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Logging.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Common.Caching.ContainerRegistrations.RegisterFor(builder);
    }
}
