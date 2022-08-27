using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Amazon.Destinations
{
    public class AmazonDestinationResolver : IDestinationResolver
    {
        private readonly IAmazonDestinationFactory _amazonDestinationFactory;

        public AmazonDestinationResolver(
            IAmazonDestinationFactory amazonDestinationFactory)
        {
            _amazonDestinationFactory = amazonDestinationFactory;
        }

        public bool CanResolve(DestinationConfig destinationConfig)
        {
            return string.Equals(destinationConfig.DestinationType, "Amazon");
        }

        public IDestination GetDestination(DestinationConfig destinationConfig)
        {
            return _amazonDestinationFactory(destinationConfig);
        }
    }
}
