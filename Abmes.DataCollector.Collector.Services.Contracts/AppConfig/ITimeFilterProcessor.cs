namespace Abmes.DataCollector.Collector.Services.Contracts.AppConfig;

public interface ITimeFilterProcessor
{
    bool TimeFilterAccepted(string timeFilter);
}
