using System.Text.Json;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public class DataCollectionsJsonConfigProvider : IDataCollectionsJsonConfigsProvider
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public IEnumerable<DataCollectionConfig> GetDataCollectionsConfig(string json)
    {
        return JsonSerializer.Deserialize<IEnumerable<DataCollectionConfig>>(json, _jsonSerializerOptions) ?? [];
    }
}
