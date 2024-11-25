using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Destinations;

public interface IDestinationResolverProvider
{
    IDestinationResolver GetResolver(DestinationConfig destinationConfig);
}
