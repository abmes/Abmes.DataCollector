using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.Web.Library;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Shared.Data.Configuration.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Shared.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Shared.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Shared.Services.DI.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Collector.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Web.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Console.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Http.ContainerRegistrations.RegisterFor(builder);

        Services.DI.ContainerRegistrations.RegisterFor(builder);
    }
}
