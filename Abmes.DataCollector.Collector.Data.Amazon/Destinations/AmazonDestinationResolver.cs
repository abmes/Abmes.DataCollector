using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Destinations;

namespace Abmes.DataCollector.Collector.Data.Amazon.Destinations;

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
