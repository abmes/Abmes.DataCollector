using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations;

public class DestinationResolverProvider(
    IEnumerable<IDestinationResolver> destinationResolvers) : IDestinationResolverProvider
{
    public IDestinationResolver GetResolver(DestinationConfig destinationConfig)
    {
        return destinationResolvers.Where(x => x.CanResolve(destinationConfig)).Single();
    }
}
