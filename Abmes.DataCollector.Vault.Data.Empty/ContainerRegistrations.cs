using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Abmes.DataCollector.Vault.Data.Empty.Configuration;
using Autofac;

namespace Abmes.DataCollector.Vault.Data.Empty;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<EmptyConfigLocationProvider>().As<IConfigLocationProvider>();
    }
}
