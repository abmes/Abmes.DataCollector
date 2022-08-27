using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Console.Destinations;

public delegate IConsoleDestination IConsoleDestinationFactory(DestinationConfig destinationConfig);
