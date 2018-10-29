using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DataCollectionConfig
    {
        public string DataCollectionName { get; }
        public string DataGroupName { get; }
        public TimeSpan InitialDelay { get; }
        public string PrepareUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> PrepareHeaders { get; }
        public string PrepareHttpMethod { get; }
        public string PrepareFinishedPollUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> PrepareFinishedPollHeaders { get; }
        public TimeSpan PrepareFinishedPollInterval { get; }
        public TimeSpan PrepareDuration { get; }
        public string CollectFileIdentifiersUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> CollectFileIdentifiersHeaders { get; }
        public string CollectUrl { get; }
        public IEnumerable<KeyValuePair<string, string>> CollectHeaders { get; }
        public TimeSpan CollectTimeout { get; }
        public bool CollectFinishWait { get; }
        public IEnumerable<string> DestinationIds { get; }

        // constructor needed for json deserialization
        public DataCollectionConfig(
            string dataCollectionName, string dataGroupName,
            TimeSpan initialDelay,
            string prepareUrl, IEnumerable<KeyValuePair<string, string>> prepareHeaders, string prepareHttpMethod, string prepareFinishedPollUrl, IEnumerable<KeyValuePair<string, string>> prepareFinishedPollHeaders, TimeSpan prepareFinishedPollInterval, TimeSpan prepareDuration,
            string collectFileIdentifiersUrl, IEnumerable<KeyValuePair<string, string>> collectFileIdentifiersHeaders, string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, TimeSpan collectTimeout, bool collectFinishWait,
            IEnumerable<string> destinationIds)
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
            CollectTimeout = collectTimeout;
            CollectFinishWait = collectFinishWait;
            DestinationIds = destinationIds;
        }
    }
}