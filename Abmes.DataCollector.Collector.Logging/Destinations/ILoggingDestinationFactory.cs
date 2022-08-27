using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Logging.Destinations;

public delegate ILoggingDestination ILoggingDestinationFactory(IDestination destination);
