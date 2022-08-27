namespace Abmes.DataCollector.Collector.Common.Configuration;

public interface IDestinationsJsonConfigProvider
{
    IEnumerable<DestinationConfig> GetDestinationsConfig(string json);
}
