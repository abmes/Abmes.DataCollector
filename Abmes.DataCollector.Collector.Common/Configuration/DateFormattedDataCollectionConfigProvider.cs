using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DateFormattedDataCollectionConfigProvider : IDateFormattedDataCollectionConfigProvider
    {
        private readonly IDateTimeFormatter _dateTimeFormatter;
        private readonly IIdentityServiceClientInfoFactory _identityServiceClientInfoFactory;

        public DateFormattedDataCollectionConfigProvider(
            IDateTimeFormatter dateTimeFormatter,
            IIdentityServiceClientInfoFactory identityServiceClientInfoFactory)
        {
            _dateTimeFormatter = dateTimeFormatter;
            _identityServiceClientInfoFactory = identityServiceClientInfoFactory;
        }

        public DataCollectionConfig GetConfig(DataCollectionConfig config)
        {
            return 
                new DataCollectionConfig(
                    config.DataCollectionName,
                    config.DataGroupName,
                    config.InitialDelay,
                    FormatDateTime(config.PrepareUrl), 
                    config.PrepareHeaders, config.PrepareHttpMethod, config.PrepareFinishedPollUrl, config.PrepareFinishedPollHeaders, config.PrepareFinishedPollInterval, config.PrepareDuration,
                    FormatDateTime(config.CollectFileIdentifiersUrl),
                    config.CollectFileIdentifiersHeaders,
                    FormatDateTime(config.CollectUrl.Replace("[filename]", "(filename)")).Replace("(filename)", "[filename]"),
                    config.CollectHeaders, config.CollectParallelFileCount, config.CollectTimeout, config.CollectFinishWait, 
                    config.DestinationIds,
                    config.ParallelDestinationCount,
                    config.LoginName,
                    config.LoginSecret,
                    config.IdentityServiceUrl,
                    config.IdentityServiceClientId,
                    config.IdentityServiceClientSecret,
                    config.IdentityServiceScope,
                    _identityServiceClientInfoFactory
                );
        }

        private string FormatDateTime(string url)
        {
            return _dateTimeFormatter.FormatDateTime(url, @"\[", "]", DateTime.UtcNow);
        }
    }
}
