using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.Console.Destinations;

public delegate IConsoleDestination IConsoleDestinationFactory(DestinationConfig destinationConfig);
