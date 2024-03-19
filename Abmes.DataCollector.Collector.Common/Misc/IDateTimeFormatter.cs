using System.Diagnostics.CodeAnalysis;

namespace Abmes.DataCollector.Collector.Common.Misc;

public interface IDateTimeFormatter
{
    [return: NotNullIfNotNull(nameof(format))]
    string? FormatDateTime(string? format, string prefix, string suffix, DateTime dateTime);
}
