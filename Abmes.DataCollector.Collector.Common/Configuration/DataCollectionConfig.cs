using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DataCollectionConfig
    {
        public string DataCollectionName { get; set;  }
        public string DataGroupName { get; set; }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan? InitialDelay { get; set; }

        public string PrepareUrl { get; set; }
        public IEnumerable<KeyValuePair<string, string>> PrepareHeaders { get; set; }
        public string PrepareHttpMethod { get; set; }
        public string PrepareFinishedPollUrl { get; set; }
        public IEnumerable<KeyValuePair<string, string>> PrepareFinishedPollHeaders { get; set; }
        
        [JsonConverter(typeof(JsonTimeSpanConverter))] 
        public TimeSpan? PrepareFinishedPollInterval { get; set; }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan? PrepareDuration { get; set; }

        public string CollectFileIdentifiersUrl { get; set; }
        public IEnumerable<KeyValuePair<string, string>> CollectFileIdentifiersHeaders { get; set; }
        public string CollectUrl { get; set; }
        public IEnumerable<KeyValuePair<string, string>> CollectHeaders { get; set; }
        public int? CollectParallelFileCount { get; set; }

        [JsonConverter(typeof(JsonTimeSpanConverter))]
        public TimeSpan? CollectTimeout { get; set; }

        public string CollectFinishWait { get; set; }
        public IEnumerable<string> DestinationIds { get; set; }
        public int? ParallelDestinationCount { get; set; }
        public int? MaxFileCount { get; set; }

        public string LoginName { get; set; }
        public string LoginSecret { get; set; }

        public string IdentityServiceUrl { get; set; }
        public string IdentityServiceClientId { get; set; }
        public string IdentityServiceClientSecret { get; set; }
        public string IdentityServiceScope { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Values { get; set; }

        public IIdentityServiceClientInfo IdentityServiceClientInfo { get; set; }

        public DataCollectionConfig()
        {
            // needed for deserialization
        }

        // constructor needed for json deserialization
        public DataCollectionConfig(
            string dataCollectionName, string dataGroupName,
            TimeSpan? initialDelay,
            string prepareUrl, 
            IEnumerable<KeyValuePair<string, string>> prepareHeaders, 
            string prepareHttpMethod, 
            string prepareFinishedPollUrl, 
            IEnumerable<KeyValuePair<string, string>> prepareFinishedPollHeaders, 
            TimeSpan? prepareFinishedPollInterval, 
            TimeSpan? prepareDuration,
            string collectFileIdentifiersUrl, 
            IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, 
            string collectUrl, 
            IEnumerable<KeyValuePair<string, string>> collectHeaders, 
            int? collectParallelFileCount, 
            TimeSpan? collectTimeout, 
            string collectFinishWait,
            IEnumerable<string> destinationIds, 
            int? parallelDestinationCount,
            int? maxFileCount,
            string loginName, 
            string loginSecret,
            string identityServiceUrl, 
            string identityServiceClientId, 
            string identityServiceClientSecret, 
            string identityServiceScope,
            IEnumerable<KeyValuePair<string, string>> values)
        {
            DataCollectionName = dataCollectionName;
            DataGroupName = dataGroupName;
            InitialDelay = initialDelay;
            PrepareUrl = prepareUrl;
            PrepareHeaders = prepareHeaders;
            PrepareHttpMethod = prepareHttpMethod;
            PrepareFinishedPollUrl = prepareFinishedPollUrl;
            PrepareFinishedPollHeaders = prepareFinishedPollHeaders;
            PrepareFinishedPollInterval = prepareFinishedPollInterval;
            PrepareDuration = prepareDuration;
            CollectFileIdentifiersUrl = collectFileIdentifiersUrl;
            CollectFileIdentifiersHeaders = collectFileIdentifiersHeaders;
            CollectUrl = collectUrl;
            CollectHeaders = collectHeaders;
            CollectParallelFileCount = collectParallelFileCount;
            CollectTimeout = collectTimeout;
            CollectFinishWait = collectFinishWait;
            DestinationIds = destinationIds;
            ParallelDestinationCount = parallelDestinationCount;
            MaxFileCount = maxFileCount;

            LoginName = loginName;
            LoginSecret = loginSecret;

            IdentityServiceUrl = identityServiceUrl;
            IdentityServiceClientId = identityServiceClientId;
            IdentityServiceClientSecret = identityServiceClientSecret;
            IdentityServiceScope = identityServiceScope;

            Values = values;

            IdentityServiceClientInfo = new IdentityServiceClientInfo(IdentityServiceUrl, IdentityServiceClientId, IdentityServiceClientSecret, IdentityServiceScope, LoginName, LoginSecret);
        }
    }
}