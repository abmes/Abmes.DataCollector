﻿using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Collector.Web.Library;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        Abmes.DataCollector.Common.Data.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Common.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Common.Data.Azure.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Data.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Amazon.ContainerRegistrations.RegisterFor(builder, configuration);
        Abmes.DataCollector.Collector.Data.Azure.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Web.ContainerRegistrations.RegisterFor(builder);
        Abmes.DataCollector.Collector.Data.Console.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Services.ContainerRegistrations.RegisterFor(builder);

        Abmes.DataCollector.Collector.Common.ContainerRegistrations.RegisterFor(builder);
    }
}
