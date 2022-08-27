using System.Text.Json;

namespace Abmes.DataCollector.Vault.Configuration;

public class StoragesJsonConfigProvider : IStoragesJsonConfigProvider
{
    public IEnumerable<StorageConfig> GetStorageConfigs(string json)
    {
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<IEnumerable<StorageConfig>>(json, options);
    }
}
