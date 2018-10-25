using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public class DestinationProvider : IDestinationProvider
    {
        private readonly IDestinationsConfigProvider _destinationsConfigProvider;
        private readonly IConfigSetNameProvider _configSetNameProvider;
        private readonly IDestinationFactory _destinationFactory;

        public DestinationProvider(
            IDestinationsConfigProvider destinationsConfigProvider,
            IConfigSetNameProvider configSetNameProvider,
            IDestinationFactory destinationFactory)
        {
            _destinationsConfigProvider = destinationsConfigProvider;
            _configSetNameProvider = configSetNameProvider;
            _destinationFactory = destinationFactory;
        }

        public async Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken)
        {
            var destinationConfig = await GetDestinationConfigAsync(destinationId, cancellationToken);
            return _destinationFactory.GetDestination(destinationConfig);
        }

        private async Task<DestinationConfig> GetDestinationConfigAsync(string destinationId, CancellationToken cancellationToken)
        {
            var configSetName = _configSetNameProvider.GetConfigSetName();
            var destinationsConfig = await _destinationsConfigProvider.GetDestinationsConfigAsync(configSetName, cancellationToken);
            var destinationConfig = destinationsConfig.Where(x => x.DestinationId.Equals(destinationId, StringComparison.CurrentCultureIgnoreCase)).SingleOrDefault();

            if (destinationConfig == null)
            {
                throw new Exception($"Destination with id '{destinationId}' not found");
            }

            return destinationConfig;
        }
    }
}
