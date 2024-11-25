using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using System.Text.Json;

namespace Abmes.DataCollector.Vault.Services.Configuration;

public class StoragesJsonConfigProvider : IStoragesJsonConfigProvider
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public IEnumerable<StorageConfig> GetStorageConfigs(string json)
    {
        return JsonSerializer.Deserialize<IEnumerable<StorageConfig>>(json, _jsonSerializerOptions) ?? [];
    }
}
