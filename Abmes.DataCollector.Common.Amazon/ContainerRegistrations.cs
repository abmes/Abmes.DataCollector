﻿using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Amazon.Configuration;
using Abmes.DataCollector.Common.Amazon.Library.Storage;
using Abmes.DataCollector.Common.Storage;
using Autofac;

namespace Abmes.DataCollector.Common.Amazon
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");
            builder.RegisterType<AmazonCommonStorage>().As<IAmazonCommonStorage>();
        }
    }
}
