using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.Storage;

public interface IStorage
{
    StorageConfig StorageConfig { get; }
    Task<IEnumerable<IFileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken);
    Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken);
}
