using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Data.Amazon;

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
