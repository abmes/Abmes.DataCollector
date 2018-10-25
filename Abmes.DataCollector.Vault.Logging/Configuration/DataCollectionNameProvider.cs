using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Vault.Configuration;
using System;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Vault.Logging.Configuration
{
    public class DataCollectionNameProvider : IDataCollectionNameProvider
    {
        private readonly ILogger<DataCollectionNameProvider> _logger;
        private readonly IDataCollectionNameProvider _dataCollectionNameProvider;

        public DataCollectionNameProvider(ILogger<DataCollectionNameProvider> logger, IDataCollectionNameProvider dataCollectionNameProvider)
        {
            _logger = logger;
            _dataCollectionNameProvider = dataCollectionNameProvider;
        }


        public string GetDataCollectionName()
        {
            try
            {
                _logger.LogInformation("Started getting data name");

                var result = _dataCollectionNameProvider.GetDataCollectionName();

                _logger.LogInformation("Finished getting data name");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting config data name: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }
    }
}
