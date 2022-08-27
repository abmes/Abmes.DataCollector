using Abmes.DataCollector.Common.Configuration;
using Autofac;

namespace Abmes.DataCollector.Common.Logging;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<Configuration.ConfigProvider>().Named<IConfigProvider>("LoggingDecorator");
        builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("LoggingDecorator", TypedParameter.From(inner)), "base").Named<IConfigProvider>("logging");
    }
}
