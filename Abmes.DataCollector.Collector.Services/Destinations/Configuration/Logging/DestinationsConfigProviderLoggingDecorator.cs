using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Destinations.Configuration.Logging;

public class DestinationsConfigProviderLoggingDecorator(
    ILogger<DestinationsConfigProviderLoggingDecorator> logger,
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
