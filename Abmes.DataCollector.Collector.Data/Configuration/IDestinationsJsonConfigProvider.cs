namespace Abmes.DataCollector.Collector.Data.Configuration;

public interface IDestinationsJsonConfigProvider
{
    IEnumerable<DestinationConfig> GetDestinationsConfig(string json);
}
