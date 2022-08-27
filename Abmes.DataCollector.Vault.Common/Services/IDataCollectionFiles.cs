using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Vault.Services;

public interface IDataCollectionFiles
{
    Task<IEnumerable<IFileInfoData>> GetFileInfosAsync(string prefix, TimeSpan? maxAge, CancellationToken cancellationToken);
    Task<IEnumerable<IFileInfoData>> GetLatestFileInfosAsync(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetFileNamesAsync(string prefix, TimeSpan? maxAge, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken);
    Task<string> GetDownloadUrlAsync(string fileName, string storageType = default, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetDownloadUrlsAsync(string fileNamePrefix, string storageType = default, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(string storageType = default, CancellationToken cancellationToken = default);
}
