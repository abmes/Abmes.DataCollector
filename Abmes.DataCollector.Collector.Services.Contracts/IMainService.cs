namespace Abmes.DataCollector.Collector.Services.Contracts;

public interface IMainService
{
    Task<int> MainAsync(CollectorParams? collectorParams, int exitDelaySeconds, CancellationToken cancellationToken);
}
