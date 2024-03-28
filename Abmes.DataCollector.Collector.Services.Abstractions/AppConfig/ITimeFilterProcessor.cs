namespace Abmes.DataCollector.Collector.Services.Abstractions.AppConfig;

public interface ITimeFilterProcessor
{
    bool TimeFilterAccepted(string timeFilter);
}
