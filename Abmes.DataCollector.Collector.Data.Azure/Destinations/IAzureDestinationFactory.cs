using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Data.Azure.Destinations;

public delegate IAzureDestination IAzureDestinationFactory(DestinationConfig destinationConfig);
