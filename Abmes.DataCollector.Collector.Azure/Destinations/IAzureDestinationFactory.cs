using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Azure.Destinations
{
    public delegate IAzureDestination IAzureDestinationFactory(DestinationConfig destinationConfig);
}
