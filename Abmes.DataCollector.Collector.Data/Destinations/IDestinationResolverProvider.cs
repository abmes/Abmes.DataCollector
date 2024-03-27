using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Destinations;

public interface IDestinationResolverProvider
{
    IDestinationResolver GetResolver(DestinationConfig destinationConfig);
}
