using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DestinationConfig
    {
        public string DestinationId { get; }
        public string DestinationType { get; }
        public string LoginName { get; }
        public string LoginSecret { get; }
        public string Root { get; }

        // constructor needed for json deserialization
        public DestinationConfig(string destinationId, string destinationType, string loginName, string loginSecret, string root)
        {
            DestinationId = destinationId;
            DestinationType = destinationType;
            LoginName = loginName;
            LoginSecret = loginSecret;
            Root = root;
        }
    }
}