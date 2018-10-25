using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Amazon.Configuration
{
    public class DestinationsConfigProvider : IDestinationsConfigProvider
    {
        private const string DestinationsConfigFileName = "DestinationsConfig.json";

        private readonly IDestinationsJsonConfigProvider _destinationsJsonConfigProvider;
        private readonly IConfigProvider _configProvider;

        public DestinationsConfigProvider(
            IDestinationsJsonConfigProvider destinationsJsonConfigProvider,
            IConfigProvider configProvider)
        {
            _destinationsJsonConfigProvider = destinationsJsonConfigProvider;
            _configProvider = configProvider;
        }

        public async Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken)
        {
            var configBlobName = (configSetName + "/" + DestinationsConfigFileName).TrimStart('/');
            var json = await _configProvider.GetConfigContentAsync(configBlobName, cancellationToken);
            return _destinationsJsonConfigProvider.GetDestinationsConfig(json);
        }
    }
}
