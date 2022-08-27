namespace Abmes.DataCollector.Collector.Common.Misc
{
    public interface IDateTimeFormatter
    {
        string FormatDateTime(string format, string prefix, string suffix, DateTime dateTime);
    }
}
