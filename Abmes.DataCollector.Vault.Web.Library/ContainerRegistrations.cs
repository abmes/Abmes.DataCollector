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
        Abmes.DataCollector.Common.Services.DI.ContainerRegistrations.RegisterFor(builder);

        Data.AspNetCore.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Data.Amazon.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Data.FileSystem.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Vault.Services.ContainerRegistrations.RegisterFor(builder);
    }
}
