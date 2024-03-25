namespace Abmes.DataCollector.Collector.Services.Configuration;

public interface IDestinationsJsonConfigProvider
{
    IEnumerable<DestinationConfig> GetDestinationsConfig(string json);
}
