using Abmes.DataCollector.Vault.Amazon.Storage;
using Abmes.DataCollector.Vault.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Amazon
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<AmazonStorage>().Named<IStorage>("baseAmazon");
        }
    }
}
