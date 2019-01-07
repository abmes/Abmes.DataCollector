using Abmes.DataCollector.Collector.Common.Destinations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Destinations
{
    public class Destination : IDestination
    {
        private readonly ILogger<Destination> _logger;
        private readonly IDestination _destination;

        public Destination(ILogger<Destination> logger, IDestination destination)
        {
            _logger = logger;
            _destination = destination;
        }

        public DestinationConfig DestinationConfig
        {
            get => _destination.DestinationConfig;
            set
            {
                _destination.DestinationConfig = value;
            }
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            try
            {
                var actionName = (tryNo == 1) ? "Started" : "Retrying";

                _logger.LogInformation(actionName + " collecting file '{fileName}' from data '{dataCollectionName}' to destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);

                await _destination.CollectAsync(collectUrl, collectHeaders, dataCollectionName, fileName, timeout, finishWait, tryNo, cancellationToken);

                _logger.LogInformation("Finished collecting file '{fileName}' from data '{dataCollectionName}' to destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error collecting file '{fileName}' from data '{dataCollectionName}' to destination '{destinationId}': {errorMessage}", fileName, dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started garbage collecting file '{fileName}' from data '{dataCollectionName}' in destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);

                await _destination.GarbageCollectDataCollectionFileAsync(dataCollectionName, fileName, cancellationToken);

                _logger.LogInformation("Finished garbage collecting file '{fileName}' from data '{dataCollectionName}' in destination '{destinationId}'", fileName, dataCollectionName, DestinationConfig.DestinationId);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error garbage collecting file '{fileName}' from data '{dataCollectionName}' in destination '{destinationId}': {errorMessage}", fileName, dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting data '{dataCollectionName}' file names in destination '{destinationId}'", dataCollectionName, DestinationConfig.DestinationId);

                var result =  await _destination.GetDataCollectionFileNamesAsync(dataCollectionName, cancellationToken);

                _logger.LogInformation("Finished getting data '{dataCollectionName}' file names in destination '{destinationId}'", dataCollectionName, DestinationConfig.DestinationId);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting data '{dataCollectionName}' file names in destination '{destinationId}': {errorMessage}", dataCollectionName, DestinationConfig.DestinationId, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
