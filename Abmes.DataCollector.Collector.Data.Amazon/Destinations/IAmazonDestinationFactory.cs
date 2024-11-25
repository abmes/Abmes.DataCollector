using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.Amazon.Destinations;

public delegate IAmazonDestination IAmazonDestinationFactory(DestinationConfig destinationConfig);
