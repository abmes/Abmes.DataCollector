using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Destinations;

public interface IDestinationResolver
{
    IDestination GetDestination(DestinationConfig destinationConfig);
    bool CanResolve(DestinationConfig destinationConfig);
}
