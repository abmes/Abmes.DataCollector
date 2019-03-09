using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public interface IBootstrapper
    {
        string ConfigSetName { get; }
        string DataCollectionNames { get; }

        void SetConfig(string configSetName, string dataCollectionNames);
    }
}
