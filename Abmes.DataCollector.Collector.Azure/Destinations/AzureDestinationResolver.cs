using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Azure.Destinations;

public class AzureDestinationResolver : IDestinationResolver
{
    private readonly IAzureDestinationFactory _AzureDestinationFactory;

    public AzureDestinationResolver(
        IAzureDestinationFactory AzureDestinationFactory)
    {
        _AzureDestinationFactory = AzureDestinationFactory;
    }

    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return string.Equals(destinationConfig.DestinationType, "Azure");
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        return _AzureDestinationFactory(destinationConfig);
    }
}
