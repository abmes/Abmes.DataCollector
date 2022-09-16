using System.Diagnostics.CodeAnalysis;

namespace Abmes.DataCollector.Collector.Common.Misc;

public interface IDateTimeFormatter
{
    [return: NotNullIfNotNull("format")]
    string? FormatDateTime(string? format, string prefix, string suffix, DateTime dateTime);
}
