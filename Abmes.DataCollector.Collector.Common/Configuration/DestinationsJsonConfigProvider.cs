using System.Text.Json;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public class DestinationsJsonConfigProvider : IDestinationsJsonConfigProvider
{
    public IEnumerable<DestinationConfig> GetDestinationsConfig(string json)
    {
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<IEnumerable<DestinationConfig>>(json, options) ?? Enumerable.Empty<DestinationConfig>();
    }
}
