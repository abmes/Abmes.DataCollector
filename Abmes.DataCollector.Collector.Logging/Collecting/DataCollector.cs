using Abmes.DataCollector.Collector.Common.Collecting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class DataCollector : IDataCollector
    {
        private readonly ILogger<DataCollector> _logger;
        private readonly IDataCollector _dataCollector;

        public DataCollector(ILogger<DataCollector> logger, IDataCollector DataCollector)
        {
            _logger = logger;
            _dataCollector = DataCollector;
        }

        public async Task CollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started processing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    await _dataCollector.CollectDataAsync(dataCollectionConfig, cancellationToken);
                }
                finally
                {
                    watch.Stop();
                }

                _logger.LogInformation("Finished processing data '{dataCollectionName}'. Elapsed time: {elapsed}", dataCollectionConfig.DataCollectionName, watch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error processing data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started garbage collecting data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);
                await _dataCollector.GarbageCollectDataAsync(dataCollectionConfig, cancellationToken);
                _logger.LogInformation("Finished garbage collecting '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error garbage collecting data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
