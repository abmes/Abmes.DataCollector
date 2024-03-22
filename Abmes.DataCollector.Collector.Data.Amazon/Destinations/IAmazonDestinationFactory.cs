using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Data.Amazon.Destinations;

public delegate IAmazonDestination IAmazonDestinationFactory(DestinationConfig destinationConfig);
