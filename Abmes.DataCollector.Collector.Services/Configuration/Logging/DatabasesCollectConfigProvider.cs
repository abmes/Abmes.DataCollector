using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Configuration.Logging;

public class DataCollectionsConfigProvider(
    ILogger<IDataCollectionsConfigProvider> logger,
    IDataCollectionsConfigProvider dataCollectionsConfigProvider) : IDataCollectionsConfigProvider
{
    public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
    {
        var displayConfigSetName = string.IsNullOrEmpty(configSetName) ? "<default>" : configSetName;

        try
        {
            logger.LogTrace("Started getting data collections config '{configSetName}'", displayConfigSetName);

            var result = (await dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken)).ToList();

            logger.LogTrace("Finished getting data collections config '{configSetName}'", displayConfigSetName);

            logger.LogInformation("Found {count} data collections to collect", result.Count);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting data collections config '{configSetName}': {errorMessage}", displayConfigSetName, e.GetAggregateMessages());
            throw;
        }
    }
}
