namespace Abmes.DataCollector.Collector.Services.AppConfig;

public interface ITimeFilterProcessor
{
    bool TimeFilterAccepted(string timeFilter);
}
