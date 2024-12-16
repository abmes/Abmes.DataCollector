using Abmes.DataCollector.Shared;
using Abmes.DataCollector.Shared.Data.FileSystem.Storage;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Data.FileSystem.Storage;

public class FileSystemStorage(
    StorageConfig storageConfig,
    IFileSystemCommonStorage fileSystemCommonStorage)
    : IFileSystemStorage
{
    public StorageConfig StorageConfig => storageConfig;

    public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        return await Task.FromResult("unavailable");
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await fileSystemCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await fileSystemCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }
}
