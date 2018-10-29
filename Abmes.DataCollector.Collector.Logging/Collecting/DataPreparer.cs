using Abmes.DataCollector.Collector.Common.Collecting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class DataPreparer : IDataPreparer
    {
        private readonly ILogger<DataPreparer> _logger;
        private readonly IDataPreparer _dataPreparer;

        public DataPreparer(ILogger<DataPreparer> logger, IDataPreparer dataPreparer)
        {
            _logger = logger;
            _dataPreparer = dataPreparer;
        }

        public async Task PrepareDataAsync(DataCollectionConfig dataCollectionConfig, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started preparing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);
                await _dataPreparer.PrepareDataAsync(dataCollectionConfig, cancellationToken);
                _logger.LogInformation("Finished preparing data '{dataCollectionName}'", dataCollectionConfig.DataCollectionName);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error preparing data '{dataCollectionName}': {errorMessage}", dataCollectionConfig.DataCollectionName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
