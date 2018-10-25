using Abmes.DataCollector.Collector.Common.Collecting;
using System;
using Abmes.DataCollector.Collector.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Abmes.DataCollector.Utils;

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

        public async Task CollectDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started processing data '{dataCollectionName}'", DataCollectConfig.DataCollectionName);

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    await _dataCollector.CollectDataAsync(DataCollectConfig, cancellationToken);
                }
                finally
                {
                    watch.Stop();
                }

                _logger.LogInformation("Finished processing data '{dataCollectionName}'. Elapsed time: {elapsed}", DataCollectConfig.DataCollectionName, watch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error processing data '{dataCollectionName}': {errorMessage}", DataCollectConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task GarbageCollectDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started garbage collecting data '{dataCollectionName}'", DataCollectConfig.DataCollectionName);
                await _dataCollector.GarbageCollectDataAsync(DataCollectConfig, cancellationToken);
                _logger.LogInformation("Finished garbage collecting '{dataCollectionName}'", DataCollectConfig.DataCollectionName);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error garbage collecting data '{dataCollectionName}': {errorMessage}", DataCollectConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
