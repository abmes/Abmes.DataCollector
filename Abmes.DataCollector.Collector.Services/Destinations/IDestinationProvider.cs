﻿using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Destinations;

public interface IDestinationProvider
{
    Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken);
}
