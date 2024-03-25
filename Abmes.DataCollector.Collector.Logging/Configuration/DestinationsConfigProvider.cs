using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Configuration;

public class DestinationsConfigProvider(
    ILogger<IDestinationsConfigProvider> logger,
    IDestinationsConfigProvider destinationsConfigProvider) : IDestinationsConfigProvider
{
    public async Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var displayConfigSetName = string.IsNullOrEmpty(configSetName) ? "<default>" : configSetName;

        try
        {
            logger.LogTrace("Started getting destinations config '{configSetName}'", displayConfigSetName);

            var result = await destinationsConfigProvider.GetDestinationsConfigAsync(configSetName, cancellationToken);

            logger.LogTrace("Finished getting destinations config '{configSetName}'", displayConfigSetName);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting destinations config '{configSetName}': {errorMessage}", displayConfigSetName, e.GetAggregateMessages());
            throw;
        }
    }
}
