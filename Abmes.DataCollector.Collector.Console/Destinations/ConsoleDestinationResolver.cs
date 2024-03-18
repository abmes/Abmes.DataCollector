using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Console.Destinations;

public class ConsoleDestinationResolver(
    IConsoleDestinationFactory webDestinationFactory) : IDestinationResolver
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
