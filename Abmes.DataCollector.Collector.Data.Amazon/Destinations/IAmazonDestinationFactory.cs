using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Amazon.Destinations;

public delegate IAmazonDestination IAmazonDestinationFactory(DestinationConfig destinationConfig);
