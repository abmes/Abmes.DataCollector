using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Services.Destinations;

public interface IDestinationResolverProvider
{
    IDestinationResolver GetResolver(DestinationConfig destinationConfig);
}
