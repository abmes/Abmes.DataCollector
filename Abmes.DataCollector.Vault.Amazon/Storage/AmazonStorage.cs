using Abmes.DataCollector.Common.Amazon.Storage;
using Abmes.DataCollector.Vault.Configuration;
using Abmes.DataCollector.Vault.Storage;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Amazon.Storage
{
    public class AmazonStorage : IStorage
    {
        private readonly IVaultAppSettings _vaultAppSettings;
        private readonly IAmazonS3 _amazonS3;
        private readonly IAmazonCommonStorage _amazonCommonStorage;

        public StorageConfig StorageConfig { get; set; }

        public AmazonStorage(
            IVaultAppSettings vaultAppSettings,
            IAmazonS3 amazonS3,
            IAmazonCommonStorage amazonCommonStorage) 
        {
            _vaultAppSettings = vaultAppSettings;
            _amazonS3 = amazonS3;
            _amazonCommonStorage = amazonCommonStorage;
        }

        public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = StorageConfig.Root,
                Key = fileName,
                Verb = HttpVerb.GET,
                Expires = DateTime.Now.Add(_vaultAppSettings.DownloadUrlExpiry)
            };

            string result = _amazonS3.GetPreSignedURL(request);

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _amazonCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.Root, dataCollectionName, cancellationToken);
        }
    }
}
