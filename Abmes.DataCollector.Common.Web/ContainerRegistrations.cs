using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;
using Autofac;
using Abmes.DataCollector.Common.Web.Configuration;

namespace Abmes.DataCollector.Common.Web
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        }
    }
}
