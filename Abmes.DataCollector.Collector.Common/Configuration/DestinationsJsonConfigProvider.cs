using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DestinationsJsonConfigProvider : IDestinationsJsonConfigProvider
    {
        public IEnumerable<DestinationConfig> GetDestinationsConfig(string json)
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<IEnumerable<DestinationConfig>>(json, options);
        }
    }
}
