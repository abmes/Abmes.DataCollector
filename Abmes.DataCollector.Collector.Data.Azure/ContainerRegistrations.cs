﻿using Abmes.DataCollector.Collector.Data.Azure.Destinations;
using Abmes.DataCollector.Collector.Data.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.Azure;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<AzureDestination>().As<IAzureDestination>();
        builder.RegisterType<AzureDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
