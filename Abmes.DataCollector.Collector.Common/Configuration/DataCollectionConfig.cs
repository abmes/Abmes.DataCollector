using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DataCollectionConfig
    {
        public string DataCollectionName { get; }
        public string DataGroupName { get; }
        public TimeSpan? InitialDelay { get; }
        public string PrepareUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> PrepareHeaders { get; }
        public string PrepareHttpMethod { get; }
        public string PrepareFinishedPollUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> PrepareFinishedPollHeaders { get; }
        public TimeSpan? PrepareFinishedPollInterval { get; }
        public TimeSpan? PrepareDuration { get; }
        public string CollectFileIdentifiersUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> CollectFileIdentifiersHeaders { get; }
        public string CollectUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> CollectHeaders { get; }
        public int? CollectParallelFileCount { get; }
        public TimeSpan? CollectTimeout { get; }
        public bool? CollectFinishWait { get; }
        public IEnumerable<string> DestinationIds { get; }
        public int? ParallelDestinationCount { get; }

        public string LoginName { get; }
        public string LoginSecret { get; }

        public string IdentityServiceUrl { get; }
        public string IdentityServiceClientId { get; }
        public string IdentityServiceClientSecret { get; }
        public string IdentityServiceScope { get; }

        public IEnumerable<KeyValuePair<string, string>> Values { get; }

        public IIdentityServiceClientInfo IdentityServiceClientInfo { get; }

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
            bool? collectFinishWait,
            IEnumerable<string> destinationIds, 
            int? parallelDestinationCount,
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