using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Destinations;

public class DestinationResolverProvider(
    IEnumerable<IDestinationResolver> destinationResolvers) : IDestinationResolverProvider
{
    public IDestinationResolver GetResolver(DestinationConfig destinationConfig)
    {
        return destinationResolvers.Where(x => x.CanResolve(destinationConfig)).Single();
    }
}
