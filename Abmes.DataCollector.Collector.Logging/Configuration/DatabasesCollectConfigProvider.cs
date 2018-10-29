using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Configuration
{
    public class DataCollectionsConfigProvider : IDataCollectionsConfigProvider
    {
        private readonly ILogger<IDataCollectionsConfigProvider> _logger;
        private readonly IDataCollectionsConfigProvider _dataCollectionsConfigProvider;

        public DataCollectionsConfigProvider(ILogger<IDataCollectionsConfigProvider> logger, IDataCollectionsConfigProvider DataCollectionsConfigProvider)
        {
            _logger = logger;
            _dataCollectionsConfigProvider = DataCollectionsConfigProvider;
        }

        public async Task<IEnumerable<DataCollectionConfig>> GetDataCollectionsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var displayConfigSetName = configSetName ?? "<default>";

            try
            {
                _logger.LogTrace("Started getting datas collect config '{configSetName}'", displayConfigSetName);

                var result = (await _dataCollectionsConfigProvider.GetDataCollectionsConfigAsync(configSetName, cancellationToken)).ToList();

                _logger.LogTrace("Finished getting datas collect config '{configSetName}'", displayConfigSetName);

                _logger.LogInformation("Found {count} datas to collect", result.Count());

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting datas collect config '{configSetName}': {errorMessage}", displayConfigSetName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
