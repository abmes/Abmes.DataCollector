using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class StoragesJsonConfigProvider : IStoragesJsonConfigProvider
    {
        public IEnumerable<StorageConfig> GetStorageConfigs(string json)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<IEnumerable<StorageConfig>>(json, options);
        }
    }
}
