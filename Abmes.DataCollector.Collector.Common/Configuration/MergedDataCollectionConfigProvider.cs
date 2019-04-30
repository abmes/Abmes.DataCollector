using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class MergedDataCollectionConfigProvider : IMergedDataCollectionConfigProvider
    {
        public DataCollectionConfig GetConfig(DataCollectionConfig config, DataCollectionConfig template)
        {
            return
                new DataCollectionConfig(
                    MergeStringValue(config.DataCollectionName, template.DataCollectionName, config),
                    MergeStringValue(config.DataGroupName, template.DataGroupName, config),
                    config.InitialDelay ?? template.InitialDelay,
                    MergeStringValue(config.PrepareUrl, template.PrepareUrl, config),
                    MergeHeaders(config.PrepareHeaders, template.PrepareHeaders, config),
                    MergeStringValue(config.PrepareHttpMethod, template.PrepareHttpMethod, config),
                    MergeStringValue(config.PrepareFinishedPollUrl, template.PrepareFinishedPollUrl, config),
                    MergeHeaders(config.PrepareFinishedPollHeaders, template.PrepareFinishedPollHeaders, config),
                    config.PrepareFinishedPollInterval ?? template.PrepareFinishedPollInterval, 
                    config.PrepareDuration ?? template.PrepareDuration,
                    MergeStringValue(config.CollectFileIdentifiersUrl, template.CollectFileIdentifiersUrl, config),
                    MergeHeaders(config.CollectFileIdentifiersHeaders, template.CollectFileIdentifiersHeaders, config),
                    MergeStringValue(config.CollectUrl, template.CollectUrl, config),
                    MergeHeaders(config.CollectHeaders, template.CollectHeaders, config), 
                    config.CollectParallelFileCount ?? template.CollectParallelFileCount, 
                    config.CollectTimeout ?? template.CollectTimeout, 
                    config.CollectFinishWait ?? template.CollectFinishWait,
                    (config.DestinationIds == null) || (!config.DestinationIds.Any()) ? template.DestinationIds : config.DestinationIds,
                    config.ParallelDestinationCount ?? template.ParallelDestinationCount,
                    config.MaxFileCount?? template.MaxFileCount,
                    MergeStringValue(config.LoginName, template.LoginName, config),
                    MergeStringValue(config.LoginSecret, template.LoginSecret, config),
                    MergeStringValue(config.IdentityServiceUrl, template.IdentityServiceUrl, config),
                    MergeStringValue(config.IdentityServiceClientId, template.IdentityServiceClientId, config),
                    MergeStringValue(config.IdentityServiceClientSecret, template.IdentityServiceClientSecret, config),
                    MergeStringValue(config.IdentityServiceScope, template.IdentityServiceScope, config),
                    config.Values
                );
        }

        private IEnumerable<KeyValuePair<string, string>> GetValues(DataCollectionConfig config)
        {
            var result = config.Values ?? Enumerable.Empty<KeyValuePair<string, string>>();

            return
                result.Concat(
                    new[] {
                        new KeyValuePair<string, string>("DataCollectionName", config.DataCollectionName),
                        new KeyValuePair<string, string>("DataGroupName", config.DataGroupName),
                        new KeyValuePair<string, string>("LoginName", config.LoginName),
                        new KeyValuePair<string, string>("LoginSecret", config.LoginSecret)
                    });
        }

        private string MergeStringValue(string value, DataCollectionConfig config)
        {
            Contract.Assert(config != null);

            var result = value;

            if (!string.IsNullOrEmpty(result))
            {
                foreach (var v in GetValues(config))
                {
                    result = result.Replace($"[{v.Key}]", v.Value);
                }
            }

            return result;
        }

        private string MergeStringValue(string value, string templateValue, DataCollectionConfig config)
        {
            var result = string.IsNullOrEmpty(value) ? templateValue : value;
            return MergeStringValue(result, config);
        }

        private IEnumerable<KeyValuePair<string, string>> MergeHeaders(IEnumerable<KeyValuePair<string, string>> headers, IEnumerable<KeyValuePair<string, string>> templateHeaders, DataCollectionConfig config)
        {
            headers = headers ?? Enumerable.Empty<KeyValuePair<string, string>>();
            templateHeaders = templateHeaders ?? Enumerable.Empty<KeyValuePair<string, string>>();

            var headerKeys = headers.Select(y => y.Key).ToList();

            var result =
                    templateHeaders
                    .Where(x => !headerKeys.Contains(x.Key))
                    .Concat(headers);

            return result?.Select(x => new KeyValuePair<string, string>(MergeStringValue(x.Key, config), MergeStringValue(x.Value, config)));
        }
    }
}
