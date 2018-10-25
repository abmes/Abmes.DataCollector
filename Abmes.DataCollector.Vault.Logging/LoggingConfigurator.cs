using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Logging
{
    public static class LoggingConfigurator
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public static void Configure(ILoggerFactory loggerFactory)
        {
        }
    }
}
