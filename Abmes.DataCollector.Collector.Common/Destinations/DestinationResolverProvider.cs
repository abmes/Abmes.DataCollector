using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public class DestinationResolverProvider : IDestinationResolverProvider
    {
        private readonly IEnumerable<IDestinationResolver> _destinationResolvers;

        public DestinationResolverProvider(IEnumerable<IDestinationResolver> destinationResolvers)
        {
            _destinationResolvers = destinationResolvers;
        }

        public IDestinationResolver GetResolver(DestinationConfig destinationConfig)
        {
            return _destinationResolvers.Where(x => x.CanResolve(destinationConfig)).Single();
        }
    }
}
