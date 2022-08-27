using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
