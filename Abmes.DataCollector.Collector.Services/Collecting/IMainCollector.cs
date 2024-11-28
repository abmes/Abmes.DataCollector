namespace Abmes.DataCollector.Collector.Services.Collecting;

public interface IMainCollector
{
    Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken);
}
