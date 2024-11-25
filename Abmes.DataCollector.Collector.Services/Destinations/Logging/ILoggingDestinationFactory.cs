using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Destinations.Logging;

public delegate ILoggingDestination ILoggingDestinationFactory(IDestination destination);
