namespace Abmes.DataCollector.Collector.Services.Abstractions.Misc;

public interface ITimeFilterProcessor
{
    bool TimeFilterAccepted(string timeFilter);
}
