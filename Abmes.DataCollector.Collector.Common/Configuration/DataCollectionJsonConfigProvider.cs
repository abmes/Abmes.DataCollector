using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DatasCollectJsonConfigProvider : IDataCollectJsonConfigsProvider
    {
        public IEnumerable<DataCollectionConfig> GetDataCollectionsConfig(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<DataCollectionConfig>>(json);
        }
    }
}
