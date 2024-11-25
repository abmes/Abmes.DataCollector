using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Configuration.ConfigSetName.Logging;

public class ConfigSetNameProviderLoggingDecorator(
    ILogger<ConfigSetNameProviderLoggingDecorator> logger,
    IConfigSetNameProvider configSetNameProvider) : IConfigSetNameProvider
{
    public string GetConfigSetName()
    {
        try
        {
            logger.LogTrace("Started getting config set name");

            var result = configSetNameProvider.GetConfigSetName();

            logger.LogTrace("Finished getting config set name");

            logger.LogInformation("Using config set name '{result}'", result);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting config set name: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }
}
