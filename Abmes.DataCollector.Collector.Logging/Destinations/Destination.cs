using Abmes.DataCollector.Collector.Common.Destinations;
using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Destinations;

public class Destination : ILoggingDestination
{
    private readonly IDestination _destination;
    private readonly ILogger<Destination> _logger;

    public Destination(IDestination destination, ILogger<Destination> logger)
    {
        _destination = destination;
        _logger = logger;
    }

    public DestinationConfig DestinationConfig
    {
        get => _destination.DestinationConfig;
    }

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken)
    {
        return _destination.AcceptsFileAsync(dataCollectionName, name, size, md5, cancellationToken);
    }

    public bool CanGarbageCollect()
    {
        return _destination.CanGarbageCollect();
    }

    public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
    {
        try
        {
            var actionName = (tryNo == 1) ? "Started" : "Retrying";

            _logger.LogInformation(actionName + " collecting data '{fileName}' from data collection '{dataCollectionName}' to destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);

            await _destination.CollectAsync(collectUrl, collectHeaders, collectIdentityServiceClientInfo, dataCollectionName, fileName, timeout, finishWait, tryNo, cancellationToken);

            _logger.LogInformation("Finished collecting data '{fileName}' from data collection '{dataCollectionName}' to destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error collecting data '{fileName}' from data collection '{dataCollectionName}' to destination '{destinationId}': {errorMessage}", fileName, dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Started garbage collecting data '{fileName}' from data collection '{dataCollectionName}' in destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);

            await _destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, fileName, cancellationToken);

            _logger.LogInformation("Finished garbage collecting data '{fileName}' from data collection '{dataCollectionName}' in destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error garbage collecting data '{fileName}' from data collection '{dataCollectionName}' in destination '{destinationId}': {errorMessage}", fileName, dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Started getting data collection '{dataCollectionName}' file names in destination '{destinationId}'", dataCollectionName, DestinationConfig.DestinationId);

            var result =  await _destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);

            _logger.LogInformation("Finished getting data collection '{dataCollectionName}' file names in destination '{destinationId}'", dataCollectionName, DestinationConfig.DestinationId);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Error getting data collection '{dataCollectionName}' file names in destination '{destinationId}': {errorMessage}", dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
            throw;
        }
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        await _destination.PutFileAsync(dataCollectionName, fileName, content, cancellationToken);
    }
}
