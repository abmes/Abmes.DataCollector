namespace Abmes.DataCollector.Collector.Services.Ports.Destinations;

public interface IDestinationResolver
{
    IDestination GetDestination(DestinationConfig destinationConfig);
    bool CanResolve(DestinationConfig destinationConfig);
}
