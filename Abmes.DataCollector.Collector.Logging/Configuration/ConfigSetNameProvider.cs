using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Configuration;

public class ConfigSetNameProvider(
    ILogger<ConfigSetNameProvider> logger,
    IConfigSetNameProvider configSetNameProvider) : IConfigSetNameProvider
{
    public string GetConfigSetName()
    {
        try
        {
            logger.LogTrace("Started getting config set name");

            var result = configSetNameProvider.GetConfigSetName();

            logger.LogTrace("Finished getting config set name");

            logger.LogInformation($"Using config set name '{result}'");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting config set name: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }
}
