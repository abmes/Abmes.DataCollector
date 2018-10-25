using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Configuration
{
    public interface IDestinationsJsonConfigProvider
    {
        IEnumerable<DestinationConfig> GetDestinationsConfig(string json);
    }
}
