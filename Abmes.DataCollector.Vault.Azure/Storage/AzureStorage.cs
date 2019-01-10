using Abmes.DataCollector.Common.Azure.Storage;
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
    public class AzureStorage : IStorage
    {
        private readonly IVaultAppSettings _vaultAppSettings;
        private readonly IAzureCommonStorage _azureCommonStorage;

        public StorageConfig StorageConfig { get; set; }

        public AzureStorage(
            IVaultAppSettings vaultAppSettings,
            IAzureCommonStorage azureCommonStorage)
        {
            _vaultAppSettings = vaultAppSettings;
            _azureCommonStorage = azureCommonStorage;
        }

        public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var container = await _azureCommonStorage.GetContainerAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), false, cancellationToken);

            var blobName = StorageConfig.RootDir('/', true) + dataCollectionName + '/' + fileName;
            var blob = container.GetBlobReference(blobName);

            var sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.Now.AddMinutes(-1);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.Now.Add(_vaultAppSettings.DownloadUrlExpiry);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            var sasToken = blob.GetSharedAccessSignature(sasConstraints);

            return blob.Uri + sasToken;
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await _azureCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
        }
    }
}
