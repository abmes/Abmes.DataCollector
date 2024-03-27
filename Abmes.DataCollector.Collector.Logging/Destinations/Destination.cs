using Abmes.DataCollector.Collector.Data.Configuration;
using Abmes.DataCollector.Collector.Data.Destinations;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Logging.Destinations;

public class Destination(
    IDestination destination,
    ILogger<Destination> logger) : ILoggingDestination
{
    public DestinationConfig DestinationConfig => destination.DestinationConfig;

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken)
    {
        return destination.AcceptsFileAsync(dataCollectionName, name, size, md5, cancellationToken);
    }

    public bool CanGarbageCollect()
    {
        return destination.CanGarbageCollect();
    }

    public async Task CollectAsync(
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? collectIdentityServiceClientInfo,
        string dataCollectionName,
        string fileName,
        TimeSpan timeout,
        bool finishWait,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started collecting data '{fileName}' from data collection '{dataCollectionName}' to destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);

            await destination.CollectAsync(collectUrl, collectHeaders, collectIdentityServiceClientInfo, dataCollectionName, fileName, timeout, finishWait, cancellationToken);

            logger.LogInformation("Finished collecting data '{fileName}' from data collection '{dataCollectionName}' to destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);
        }
        catch (Exception e)
        {
            logger.LogCritical("Error collecting data '{fileName}' from data collection '{dataCollectionName}' to destination '{destinationId}': {errorMessage}", fileName, dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started garbage collecting data '{fileName}' from data collection '{dataCollectionName}' in destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);

            await destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, fileName, cancellationToken);

            logger.LogInformation("Finished garbage collecting data '{fileName}' from data collection '{dataCollectionName}' in destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);
        }
        catch (Exception e)
        {
            logger.LogCritical("Error garbage collecting data '{fileName}' from data collection '{dataCollectionName}' in destination '{destinationId}': {errorMessage}", fileName, dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Started getting data collection '{dataCollectionName}' file names in destination '{destinationId}'", dataCollectionName, DestinationConfig.DestinationId);

            var result =  await destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);

            logger.LogInformation("Finished getting data collection '{dataCollectionName}' file names in destination '{destinationId}'", dataCollectionName, DestinationConfig.DestinationId);

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical("Error getting data collection '{dataCollectionName}' file names in destination '{destinationId}': {errorMessage}", dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        await destination.PutFileAsync(dataCollectionName, fileName, content, cancellationToken);
    }
}
