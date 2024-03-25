using Abmes.DataCollector.Collector.Services.Destinations;

namespace Abmes.DataCollector.Collector.Logging.Destinations;

public delegate ILoggingDestination ILoggingDestinationFactory(IDestination destination);
