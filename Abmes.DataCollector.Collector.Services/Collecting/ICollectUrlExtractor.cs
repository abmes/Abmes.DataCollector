namespace Abmes.DataCollector.Collector.Services.Collecting;

public interface ICollectUrlExtractor
{
    Task<string> ExtractCollectUrlAsync(string dataCollectionName, string? collectFileIdentifier, string sourceUrl, IEnumerable<KeyValuePair<string, string>> headers, string? identityServiceAccessToken, CancellationToken cancellationToken);
}
