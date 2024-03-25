using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Data.Azure.Destinations;

public delegate IAzureDestination IAzureDestinationFactory(DestinationConfig destinationConfig);
