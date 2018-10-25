using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class StorageJsonConfigsProvider : IStorageJsonConfigsProvider
    {
        public IEnumerable<StorageConfig> GetStorageConfigs(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<StorageConfig>>(json);
        }
    }
}
