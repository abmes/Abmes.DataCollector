namespace Abmes.DataCollector.Collector.Common.Collecting;

public interface ISimpleContentProvider
{
    Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken);
}
