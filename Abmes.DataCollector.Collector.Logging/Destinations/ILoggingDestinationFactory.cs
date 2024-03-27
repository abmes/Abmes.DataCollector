using Abmes.DataCollector.Collector.Data.Destinations;

namespace Abmes.DataCollector.Collector.Logging.Destinations;

public delegate ILoggingDestination ILoggingDestinationFactory(IDestination destination);
