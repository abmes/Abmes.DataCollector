using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Azure.Destinations;

public delegate IAzureDestination IAzureDestinationFactory(DestinationConfig destinationConfig);
