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
        public static void Configure(ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            var configSection = configuration.GetAWSLoggingConfigSection();

            if (!string.IsNullOrEmpty(configSection?.Config?.LogGroup))
            {
                configSection.Config.ProfilesLocation = configuration.GetSection("AWS.Logging")?.GetValue<string>("ProfilesLocation");
                loggingBuilder.AddAWSProvider(configSection);
            }
        }
    }
}
