﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    public static class ServicesConfiguration
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            Abmes.DataCollector.Common.ServicesConfiguration.Configure(services, configuration);
            Abmes.DataCollector.Common.Amazon.ServicesConfiguration.Configure(services, configuration);
            Abmes.DataCollector.Common.Azure.ServicesConfiguration.Configure(services, configuration);
            Abmes.DataCollector.Common.FileSystem.ServicesConfiguration.Configure(services, configuration);
        }
    }
}
