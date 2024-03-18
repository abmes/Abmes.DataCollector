using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Amazon.Destinations;

public class AmazonDestinationResolver(
    IAmazonDestinationFactory amazonDestinationFactory) : IDestinationResolver
{
    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return string.Equals(destinationConfig.DestinationType, "Amazon");
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        return amazonDestinationFactory(destinationConfig);
    }
}
