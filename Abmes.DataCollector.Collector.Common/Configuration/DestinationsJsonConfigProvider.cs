using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DestinationsJsonConfigProvider : IDestinationsJsonConfigProvider
    {
        public IEnumerable<DestinationConfig> GetDestinationsConfig(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<DestinationConfig>>(json);
        }
    }
}
