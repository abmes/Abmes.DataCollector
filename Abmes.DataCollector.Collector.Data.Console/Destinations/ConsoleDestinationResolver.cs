using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.Console.Destinations;

public class ConsoleDestinationResolver(
    IConsoleDestinationFactory webDestinationFactory)
    : IDestinationResolver
{
    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return string.Equals(destinationConfig.DestinationType, "Console");
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        return webDestinationFactory(destinationConfig);
    }
}
