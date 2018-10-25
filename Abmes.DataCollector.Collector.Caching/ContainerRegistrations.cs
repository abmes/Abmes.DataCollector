using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Abmes.DataCollector.Collector.Caching.Cache;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Collector.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Caching
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigFileCache>().As<IConfigFileCache>().SingleInstance();

            builder.RegisterType<Caching.Configuration.ConfigFileProvider>().Named<IConfigProvider>("CachingDecorator");

            builder.RegisterDecorator<IConfigProvider>((x, inner) => x.ResolveNamed<IConfigProvider>("CachingDecorator", TypedParameter.From(inner)), "logging").As<IConfigProvider>();
        }
    }
}
