using Autofac;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Services;
using Abmes.DataCollector.Vault.Storage;

namespace Abmes.DataCollector.Vault.Service
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            Abmes.DataCollector.Common.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Common.Amazon.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Common.Azure.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Vault.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Vault.Logging.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Vault.Amazon.ContainerRegistrations.RegisterFor(builder);
            Abmes.DataCollector.Vault.Azure.ContainerRegistrations.RegisterFor(builder);
        }
    }
}
