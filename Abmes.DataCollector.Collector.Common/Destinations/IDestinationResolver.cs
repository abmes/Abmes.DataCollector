using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations;

public interface IDestinationResolver
{
    IDestination GetDestination(DestinationConfig destinationConfig);
    bool CanResolve(DestinationConfig destinationConfig);
}
