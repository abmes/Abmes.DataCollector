namespace Abmes.DataCollector.Collector.Data.Destinations;

public interface IDestinationProvider
{
    Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken);
}
