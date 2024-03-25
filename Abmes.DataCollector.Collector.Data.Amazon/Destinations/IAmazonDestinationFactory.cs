using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Data.Amazon.Destinations;

public delegate IAmazonDestination IAmazonDestinationFactory(DestinationConfig destinationConfig);
