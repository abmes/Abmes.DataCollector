namespace Abmes.DataCollector.Collector.Services.Abstractions;

public interface IMainCollector
{
    Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken);
}
