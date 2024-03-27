namespace Abmes.DataCollector.Collector.Data.Configuration;

public interface IDestinationsConfigProvider
{
    Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken);
}
