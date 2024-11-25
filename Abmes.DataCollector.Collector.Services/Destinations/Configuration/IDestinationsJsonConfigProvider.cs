using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Services.Destinations.Configuration;

public interface IDestinationsJsonConfigProvider
{
    IEnumerable<DestinationConfig> GetDestinationsConfig(string json);
}
