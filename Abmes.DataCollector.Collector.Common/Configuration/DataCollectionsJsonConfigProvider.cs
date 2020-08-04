using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DataCollectionsJsonConfigProvider : IDataCollectionsJsonConfigsProvider
    {
        public IEnumerable<DataCollectionConfig> GetDataCollectionsConfig(string json)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<IEnumerable<DataCollectionConfig>>(json, options);
        }
    }
}
