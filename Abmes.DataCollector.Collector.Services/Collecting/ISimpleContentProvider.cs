namespace Abmes.DataCollector.Collector.Services.Collecting;

public interface ISimpleContentProvider
{
    Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken);
}
