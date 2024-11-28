using Abmes.DataCollector.Collector.Services.Misc;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Abmes.DataCollector.Collector.Services.Configuration;

public class DateFormattedDataCollectionConfigProvider(
    TimeProvider timeProvider,
    IDateTimeFormatter dateTimeFormatter) : IDateFormattedDataCollectionConfigProvider
{
    public DataCollectionConfig GetConfig(DataCollectionConfig config)
    {
        return
            new DataCollectionConfig
            {
                DataCollectionName = config.DataCollectionName,
                DataGroupName = config.DataGroupName,
                InitialDelay = config.InitialDelay,
                PrepareUrl = FormatDateTime(config.PrepareUrl),
                PrepareHeaders = config.PrepareHeaders,
                PrepareHttpMethod = config.PrepareHttpMethod,
                PrepareFinishedPollUrl = config.PrepareFinishedPollUrl,
                PrepareFinishedPollHeaders = config.PrepareFinishedPollHeaders,
                PrepareFinishedPollInterval = config.PrepareFinishedPollInterval,
                PrepareDuration = config.PrepareDuration,
                CollectFileIdentifiersUrl = FormatDateTime(config.CollectFileIdentifiersUrl),
                CollectFileIdentifiersHeaders = config.CollectFileIdentifiersHeaders,
                CollectUrl = FormatDateTime(config.CollectUrl?.Replace("[filename]", "(filename)"))?.Replace("(filename)", "[filename]"),
                CollectHeaders = config.CollectHeaders,
                CollectParallelFileCount = config.CollectParallelFileCount,
                CollectTimeout = config.CollectTimeout,
                CollectFinishWait = config.CollectFinishWait,
                DestinationIds = config.DestinationIds,
                ParallelDestinationCount = config.ParallelDestinationCount,
                MaxFileCount = config.MaxFileCount,
                LoginName = config.LoginName,
                LoginSecret = config.LoginSecret,
                IdentityServiceUrl = config.IdentityServiceUrl,
                IdentityServiceClientId = config.IdentityServiceClientId,
                IdentityServiceClientSecret = config.IdentityServiceClientSecret,
                IdentityServiceScope = config.IdentityServiceScope,
                Values = config.Values
            };
    }

    [return: NotNullIfNotNull(nameof(url))]
    private string? FormatDateTime(string? url)
    {
        return dateTimeFormatter.FormatDateTime(url, @"\[", "]", timeProvider.GetUtcNow().DateTime);
    }
}
