using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    public static class ServicesConfiguration
    {
        public static void Configure(IServiceCollection services, IConfigurationRoot configuration)
        {
            Abmes.DataCollector.Common.Amazon.ServicesConfiguration.Configure(services, configuration);
        }
    }
}
