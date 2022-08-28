using System.Diagnostics.Contracts;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public class MergedDataCollectionConfigProvider : IMergedDataCollectionConfigProvider
{
    public DataCollectionConfig GetConfig(DataCollectionConfig config, DataCollectionConfig template)
    {
        return new DataCollectionConfig(
            DataCollectionName: MergeStringValue(config.DataCollectionName, template.DataCollectionName, config),
            DataGroupName: MergeStringValue(config.DataGroupName, template.DataGroupName, config),
            InitialDelay: config.InitialDelay ?? template.InitialDelay,
            PrepareUrl: MergeStringValue(config.PrepareUrl, template.PrepareUrl, config),
            PrepareHeaders: MergeHeaders(config.PrepareHeaders, template.PrepareHeaders, config),
            PrepareHttpMethod: MergeStringValue(config.PrepareHttpMethod, template.PrepareHttpMethod, config),
            PrepareFinishedPollUrl: MergeStringValue(config.PrepareFinishedPollUrl, template.PrepareFinishedPollUrl, config),
            PrepareFinishedPollHeaders: MergeHeaders(config.PrepareFinishedPollHeaders, template.PrepareFinishedPollHeaders, config),
            PrepareFinishedPollInterval: config.PrepareFinishedPollInterval ?? template.PrepareFinishedPollInterval,
            PrepareDuration: config.PrepareDuration ?? template.PrepareDuration,
            CollectFileIdentifiersUrl: MergeStringValue(config.CollectFileIdentifiersUrl, template.CollectFileIdentifiersUrl, config),
            CollectFileIdentifiersHeaders: MergeHeaders(config.CollectFileIdentifiersHeaders, template.CollectFileIdentifiersHeaders, config),
            CollectUrl: MergeStringValue(config.CollectUrl, template.CollectUrl, config),
            CollectHeaders: MergeHeaders(config.CollectHeaders, template.CollectHeaders, config),
            CollectParallelFileCount: config.CollectParallelFileCount ?? template.CollectParallelFileCount,
            CollectTimeout: config.CollectTimeout ?? template.CollectTimeout,
            CollectFinishWait: config.CollectFinishWait ?? template.CollectFinishWait,
            DestinationIds: (config.DestinationIds == null) || (!config.DestinationIds.Any()) ? template.DestinationIds : config.DestinationIds,
            ParallelDestinationCount: config.ParallelDestinationCount ?? template.ParallelDestinationCount,
            MaxFileCount: config.MaxFileCount?? template.MaxFileCount,
            LoginName: MergeStringValue(config.LoginName, template.LoginName, config),
            LoginSecret: MergeStringValue(config.LoginSecret, template.LoginSecret, config),
            IdentityServiceUrl: MergeStringValue(config.IdentityServiceUrl, template.IdentityServiceUrl, config),
            IdentityServiceClientId: MergeStringValue(config.IdentityServiceClientId, template.IdentityServiceClientId, config),
            IdentityServiceClientSecret: MergeStringValue(config.IdentityServiceClientSecret, template.IdentityServiceClientSecret, config),
            IdentityServiceScope: MergeStringValue(config.IdentityServiceScope, template.IdentityServiceScope, config),
            Values: config.Values
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
