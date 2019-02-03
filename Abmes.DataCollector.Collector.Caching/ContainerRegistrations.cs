using Autofac;
using Abmes.DataCollector.Collector.Caching.Cache;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Caching
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigFileCache>().As<IConfigFileCache>().SingleInstance();

            builder.RegisterType<Caching.Configuration.ConfigFileProvider>().Named<IConfigProvider>("CachingDecorator");
            builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "loggingAmazon").As<IConfigProvider>();
            //builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "loggingAzure").As<IConfigProvider>();

            builder.RegisterType<Caching.Configuration.ConfigSetNameProvider>().Named<IConfigSetNameProvider>("CachingDecorator");
            builder.RegisterDecorator<IConfigSetNameProvider>((x, inner) => x.ResolveNamed<IConfigSetNameProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigSetNameProvider>().SingleInstance();
        }
    }
}
