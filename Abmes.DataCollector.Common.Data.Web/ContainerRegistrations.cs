using Abmes.DataCollector.Common.Data.Configuration;
using Autofac;
using Abmes.DataCollector.Common.Data.Web.Configuration;

namespace Abmes.DataCollector.Common.Data.Web;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
    }
}
