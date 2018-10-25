using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DateFormattedDataCollectConfigProvider : IDateFormattedDataCollectConfigProvider
    {
        private readonly IDateTimeFormatter _dateTimeFormatter;

        public DateFormattedDataCollectConfigProvider(
            IDateTimeFormatter dateTimeFormatter)
        {
            _dateTimeFormatter = dateTimeFormatter;
        }

        public DataCollectConfig GetConfig(DataCollectConfig config)
        {
            return 
                new DataCollectConfig(
                    config.DataCollectionName,
                    config.DataGroupName,
                    config.InitialDelay,
                    FormatDateTime(config.PrepareUrl), 
                    config.PrepareHeaders, config.PrepareHttpMethod, config.PrepareFinishedPollUrl, config.PrepareFinishedPollHeaders, config.PrepareFinishedPollInterval, config.PrepareDuration,
                    FormatDateTime(config.CollectFileIdentifiersUrl),
                    config.CollectFileIdentifiersHeaders,
                    FormatDateTime(config.CollectUrl.Replace("[filename]", "(filename)")).Replace("(filename)", "[filename]"),
                    config.CollectHeaders, config.CollectTimeout, config.CollectFinishWait, 
                    config.DestinationIds
                );
        }

        private string FormatDateTime(string url)
        {
            return _dateTimeFormatter.FormatDateTime(url, @"\[", "]", DateTime.UtcNow);
        }
    }
}
