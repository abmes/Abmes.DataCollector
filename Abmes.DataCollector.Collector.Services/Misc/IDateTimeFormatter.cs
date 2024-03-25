using System.Diagnostics.CodeAnalysis;

namespace Abmes.DataCollector.Collector.Services.Misc;

public interface IDateTimeFormatter
{
    [return: NotNullIfNotNull(nameof(format))]
    string? FormatDateTime(string? format, string prefix, string suffix, DateTime dateTime);
}
