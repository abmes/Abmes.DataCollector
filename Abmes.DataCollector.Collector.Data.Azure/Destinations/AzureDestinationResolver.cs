using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Data.Azure.Destinations;

public class AzureDestinationResolver(
    IAzureDestinationFactory AzureDestinationFactory) : IDestinationResolver
{
    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return string.Equals(destinationConfig.DestinationType, "Azure");
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        return AzureDestinationFactory(destinationConfig);
    }
}
