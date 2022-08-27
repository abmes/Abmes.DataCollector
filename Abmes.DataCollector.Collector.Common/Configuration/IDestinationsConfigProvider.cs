namespace Abmes.DataCollector.Collector.Common.Configuration;

public interface IDestinationsConfigProvider
{
    Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken);
}
