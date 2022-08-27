namespace Abmes.DataCollector.Collector.Common.Collecting;

public interface IMainCollector
{
    Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken);
}
