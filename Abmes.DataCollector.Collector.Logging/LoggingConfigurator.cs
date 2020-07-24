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
        public static void Configure(ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            Abmes.DataCollector.Collector.Amazon.LoggingConfigurator.Configure(loggingBuilder, configuration);
        }
    }
}
