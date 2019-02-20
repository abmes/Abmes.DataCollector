using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Logging.Configuration
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly ILogger<ConfigProvider> _logger;
        private readonly IConfigProvider _configProvider;

        public ConfigProvider(ILogger<ConfigProvider> logger, IConfigProvider configProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
        }

        public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started getting config file '{fileName}' content", fileName);

                var result = await _configProvider.GetConfigContentAsync(fileName, cancellationToken);

                _logger.LogInformation("Finished getting config file '{fileName}' content", fileName);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error getting config file '{fileName}' content: {errorMessage}", fileName, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
