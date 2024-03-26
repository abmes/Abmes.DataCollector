using System.Text.Json;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class DestinationsJsonConfigProvider : IDestinationsJsonConfigProvider
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public IEnumerable<DestinationConfig> GetDestinationsConfig(string json)
    {
        return JsonSerializer.Deserialize<IEnumerable<DestinationConfig>>(json, _jsonSerializerOptions) ?? [];
    }
}
