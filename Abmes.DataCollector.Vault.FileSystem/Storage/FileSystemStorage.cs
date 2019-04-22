using Abmes.DataCollector.Common.FileSystem.Storage;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.FileSystem.Storage
{
    public class FileSystemStorage : IFileSystemStorage
    {
        private readonly IVaultAppSettings _vaultAppSettings;
        private readonly IFileSystemCommonStorage _fileSystemCommonStorage;

        public StorageConfig StorageConfig { get; }

        public FileSystemStorage(
            StorageConfig storageConfig,
            IVaultAppSettings vaultAppSettings,
            IFileSystemCommonStorage FileSystemCommonStorage) 
        {
            StorageConfig = storageConfig;
            _vaultAppSettings = vaultAppSettings;
            _fileSystemCommonStorage = FileSystemCommonStorage;
        }

        public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            return await Task.FromResult("unavailable");
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await _fileSystemCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
        }

        public async Task<IEnumerable<IFileInfo>> GetDataCollectionFileInfosAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await _fileSystemCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
        }
    }
}
