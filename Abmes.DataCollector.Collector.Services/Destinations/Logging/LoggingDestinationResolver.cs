﻿using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Destinations.Logging;

public class LoggingDestinationResolver(
    IDestinationResolver destinationResolver,
    ILoggingDestinationFactory loggingDestinationFactory)
    : IDestinationResolver
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
