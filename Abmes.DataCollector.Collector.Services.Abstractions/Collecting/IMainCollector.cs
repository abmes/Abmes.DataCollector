namespace Abmes.DataCollector.Collector.Services.Abstractions.Collecting;

public interface IMainCollector
{
    Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken);
}
