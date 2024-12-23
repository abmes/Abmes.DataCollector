﻿using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.Web.Destinations;

public class WebDestinationResolver(
    IWebDestinationFactory webDestinationFactory)
    : IDestinationResolver
{
    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return string.Equals(destinationConfig.DestinationType, "Web");
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        return webDestinationFactory(destinationConfig);
    }
}
