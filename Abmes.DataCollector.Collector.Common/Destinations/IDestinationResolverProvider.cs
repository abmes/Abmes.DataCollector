using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations;

public interface IDestinationResolverProvider
{
    IDestinationResolver GetResolver(DestinationConfig destinationConfig);
}
