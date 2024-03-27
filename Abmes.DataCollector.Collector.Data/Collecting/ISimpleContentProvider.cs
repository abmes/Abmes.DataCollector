namespace Abmes.DataCollector.Collector.Data.Collecting;

public interface ISimpleContentProvider
{
    Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken);
}
