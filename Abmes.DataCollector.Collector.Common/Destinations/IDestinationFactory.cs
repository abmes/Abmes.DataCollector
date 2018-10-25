using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestinationFactory
    {
        IDestination GetDestination(DestinationConfig destinationConfig);
    }
}
