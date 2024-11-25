namespace Abmes.DataCollector.Collector.Services.Contracts;

public interface IMainCollector
{
    Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken);
}
