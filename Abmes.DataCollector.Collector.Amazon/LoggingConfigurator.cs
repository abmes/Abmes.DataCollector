using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Amazon
{
    public static class LoggingConfigurator
    {
        public static void Configure(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            var configSection = configuration.GetAWSLoggingConfigSection();

            configSection.Config.ProfilesLocation = configuration.GetSection("AWS.Logging")?.GetValue<string>("ProfilesLocation");

            if (!string.IsNullOrEmpty(configSection?.Config?.LogGroup))
            {
                loggerFactory.AddAWSProvider(configSection);
            }
        }
    }
}
