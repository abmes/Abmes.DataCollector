using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.AmazonLambda
{
    public class CollectorParams
    {
        public string ConfigSetName { get; set; }
        public string DataCollectionNames { get; set; }
        public string CollectorMode { get; set; }
    }
}
