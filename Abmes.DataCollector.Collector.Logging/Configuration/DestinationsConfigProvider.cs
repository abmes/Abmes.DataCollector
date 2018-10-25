using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Logging.Configuration
{
    public class DestinationsConfigProvider : IDestinationsConfigProvider
    {
        private readonly ILogger<IDestinationsConfigProvider> _logger;
        private readonly IDestinationsConfigProvider _destinationsConfigProvider;

        public DestinationsConfigProvider(ILogger<IDestinationsConfigProvider> logger, IDestinationsConfigProvider destinationsConfigProvider)
        {
            _logger = logger;
            _destinationsConfigProvider = destinationsConfigProvider;
        }
        public async Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var displayConfigSetName = configSetName ?? "<default>";

            try
            {
                _logger.LogTrace("Started getting destinations config '{configSetName}'", displayConfigSetName);

                var result = await _destinationsConfigProvider.GetDestinationsConfigAsync(configSetName, cancellationToken);

                _logger.LogTrace("Finished getting destinations config '{configSetName}'", displayConfigSetName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting destinations config '{configSetName}': {errorMessage}", displayConfigSetName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
