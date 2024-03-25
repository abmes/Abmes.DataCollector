using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Data.Console.Destinations;

public delegate IConsoleDestination IConsoleDestinationFactory(DestinationConfig destinationConfig);
