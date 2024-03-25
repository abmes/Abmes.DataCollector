using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Services.Destinations;

public interface IDestinationResolver
{
    IDestination GetDestination(DestinationConfig destinationConfig);
    bool CanResolve(DestinationConfig destinationConfig);
}
