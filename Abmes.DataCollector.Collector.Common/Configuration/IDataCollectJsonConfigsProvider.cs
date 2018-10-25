using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Configuration
{
    public interface IDataCollectJsonConfigsProvider
    {
        IEnumerable<DataCollectConfig> GetDataCollectConfigs(string json);
    }
}
