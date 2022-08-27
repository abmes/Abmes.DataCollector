namespace Abmes.DataCollector.Collector.Common.Misc
{
    public interface ITimeFilterProcessor
    {
        bool TimeFilterAccepted(string timeFilter);
    }
}
