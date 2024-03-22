using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Data.Console.Destinations;

public delegate IConsoleDestination IConsoleDestinationFactory(DestinationConfig destinationConfig);
