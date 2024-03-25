namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDestinationsConfigProvider
{
    Task<IEnumerable<DestinationConfig>> GetDestinationsConfigAsync(string configSetName, CancellationToken cancellationToken);
}
