using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Configuration;

public class ConfigSetNameProvider : IConfigSetNameProvider
{
    private readonly ILogger<ConfigSetNameProvider> _logger;
    private readonly IConfigSetNameProvider _configSetNameProvider;

    public ConfigSetNameProvider(ILogger<ConfigSetNameProvider> logger, IConfigSetNameProvider configSetNameProvider)
    {
        _logger = logger;
        _configSetNameProvider = configSetNameProvider;
    }

    public string GetConfigSetName()
    {
        try
        {
            _logger.LogTrace("Started getting config set name");

            var result = _configSetNameProvider.GetConfigSetName();

            _logger.LogTrace("Finished getting config set name");

            _logger.LogInformation($"Using config set name '{result}'");

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting config set name: {errorMessage}", e.GetAggregateMessages());
            throw;
        }
    }
}
