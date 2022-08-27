namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestinationProvider
    {
        Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken);
    }
}
