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
    public class DataCollectConfigsProvider : IDataCollectConfigsProvider
    {
        private readonly ILogger<IDataCollectConfigsProvider> _logger;
        private readonly IDataCollectConfigsProvider _dataCollectConfigsProvider;

        public DataCollectConfigsProvider(ILogger<IDataCollectConfigsProvider> logger, IDataCollectConfigsProvider dataCollectConfigsProvider)
        {
            _logger = logger;
            _dataCollectConfigsProvider = dataCollectConfigsProvider;
        }

        public async Task<IEnumerable<DataCollectConfig>> GetDataCollectConfigsAsync(string configSetName, CancellationToken cancellationToken)
        {
            var displayConfigSetName = configSetName ?? "<default>";

            try
            {
                _logger.LogTrace("Started getting datas collect config '{configSetName}'", displayConfigSetName);

                var result = (await _dataCollectConfigsProvider.GetDataCollectConfigsAsync(configSetName, cancellationToken)).ToList();

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
