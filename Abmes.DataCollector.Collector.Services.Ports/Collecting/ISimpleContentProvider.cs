namespace Abmes.DataCollector.Collector.Services.Ports.Collecting;

public interface ISimpleContentProvider
{
    Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken);
}
