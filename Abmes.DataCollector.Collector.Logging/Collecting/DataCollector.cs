using Abmes.DataCollector.Collector.Common.Collecting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class DataCollector : IDataCollector
    {
        private readonly ILogger<DataCollector> _logger;
        private readonly IDataCollector _dataCollector;

        public DataCollector(ILogger<DataCollector> logger, IDataCollector dataCollector)
        {
            _logger = logger;
            _dataCollector = dataCollector;
        }

        public async Task<IEnumerable<string>> CollectDataAsync(CollectorMode collectorMode, DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<string> result;

                _logger.LogInformation("Started processing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    result = (await _dataCollector.CollectDataAsync(collectorMode, dataCollectionConfig, cancellationToken)).ToList();
                }
                finally
                {
                    watch.Stop();
                }

                _logger.LogInformation("Finished processing data '{dataCollectionName}'. Elapsed time: {elapsed}", dataCollectionConfig.DataCollectionName, watch.Elapsed);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error processing data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }

        public async Task GarbageCollectDataAsync(DataCollectionConfig dataCollectionConfig, IEnumerable<string> newFileNames, CancellationToken cancellationToken)
        {
            try
            {
                await _dataCollector.GarbageCollectDataAsync(dataCollectionConfig, newFileNames, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error garbage collecting data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
