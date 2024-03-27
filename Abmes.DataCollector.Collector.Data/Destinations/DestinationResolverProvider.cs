using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Destinations;

public class DestinationResolverProvider(
    IEnumerable<IDestinationResolver> destinationResolvers) : IDestinationResolverProvider
{
    public IDestinationResolver GetResolver(DestinationConfig destinationConfig)
    {
        return destinationResolvers.Where(x => x.CanResolve(destinationConfig)).Single();
    }
}
