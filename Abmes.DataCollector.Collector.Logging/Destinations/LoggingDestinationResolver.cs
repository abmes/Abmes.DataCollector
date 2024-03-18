using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;

namespace Abmes.DataCollector.Collector.Logging.Destinations;

public class LoggingDestinationResolver(
    IDestinationResolver destinationResolver,
    ILoggingDestinationFactory loggingDestinationFactory) : IDestinationResolver
{
    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return destinationResolver.CanResolve(destinationConfig);
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        var destination = destinationResolver.GetDestination(destinationConfig);
        return loggingDestinationFactory(destination);
    }
}
