using Abmes.DataCollector.Collector.Common.Misc;

namespace Abmes.DataCollector.Collector.Common.Configuration;

public class DateFormattedDataCollectionConfigProvider : IDateFormattedDataCollectionConfigProvider
{
    private readonly IDateTimeFormatter _dateTimeFormatter;

    public DateFormattedDataCollectionConfigProvider(
        IDateTimeFormatter dateTimeFormatter)
    {
        _dateTimeFormatter = dateTimeFormatter;
    }

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
                CollectUrl = FormatDateTime(config.CollectUrl.Replace("[filename]", "(filename)")).Replace("(filename)", "[filename]"),
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

    private string FormatDateTime(string url)
    {
        return _dateTimeFormatter.FormatDateTime(url, @"\[", "]", DateTime.UtcNow);
    }
}
