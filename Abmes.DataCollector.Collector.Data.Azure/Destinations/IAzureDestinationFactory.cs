using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.Azure.Destinations;

public delegate IAzureDestination IAzureDestinationFactory(DestinationConfig destinationConfig);
