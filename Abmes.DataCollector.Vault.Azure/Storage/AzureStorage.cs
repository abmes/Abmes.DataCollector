using Abmes.DataCollector.Common.Azure.Storage;
using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Azure.Storage
{
    public class AzureStorage : IAzureStorage
    {
        private readonly IVaultAppSettings _vaultAppSettings;
        private readonly IAzureCommonStorage _azureCommonStorage;

        public StorageConfig StorageConfig { get; }

        public AzureStorage(
            StorageConfig storageConfig,
            IVaultAppSettings vaultAppSettings,
            IAzureCommonStorage azureCommonStorage)
        {
            StorageConfig = storageConfig;
            _vaultAppSettings = vaultAppSettings;
            _azureCommonStorage = azureCommonStorage;
        }

        public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var root = string.IsNullOrEmpty(StorageConfig.Root) ? dataCollectionName : StorageConfig.RootBase();
            var container = await _azureCommonStorage.GetContainerAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, root, false, cancellationToken);

            var blobName = GetBlobName(dataCollectionName, fileName);
            var blob = container.GetBlobReference(blobName);

            var sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.Now.AddMinutes(-1);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.Now.Add(_vaultAppSettings.DownloadUrlExpiry);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            var sasToken = blob.GetSharedAccessSignature(sasConstraints);

            return blob.Uri + sasToken;
        }

        private string GetBlobName(string dataCollectionName, string fileName)
        {
            return string.IsNullOrEmpty(StorageConfig.Root) ? fileName : (StorageConfig.RootDir('/', true) + dataCollectionName + "/" + fileName);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await _azureCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
        }

        public async Task<IEnumerable<IFileInfo>> GetDataCollectionFileInfosAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await _azureCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
        }
    }
}
