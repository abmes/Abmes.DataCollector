using Abmes.DataCollector.Common.Azure.Configuration;
using Abmes.DataCollector.Common.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Azure.Storage
{
    public class AzureCommonStorage : IAzureCommonStorage
    {
        private readonly IAzureAppSettings _commonAppSettings;
        private readonly IFileInfoFactory _fileInfoFactory;

        public string StorageType => "Azure";

        public AzureCommonStorage(
            IAzureAppSettings commonAppSettings,
            IFileInfoFactory fileInfoFactory)
        {
            _commonAppSettings = commonAppSettings;
            _fileInfoFactory = fileInfoFactory;
        }

        public async Task<CloudBlobContainer> GetContainerAsync(string loginName, string loginSecret, string root, bool createIfNotExists, CancellationToken cancellationToken)
        {
            var account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(GetAzureStorageConnectionString(loginName, loginSecret));

            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(root);

            if (createIfNotExists)
            {
                await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, null, null, cancellationToken);
            }

            return container;
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return
                (await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, true, cancellationToken))
                .Select(x => x.Name);
        }

        public async Task<IEnumerable<IFileInfo>> GetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
        }

        private string GetAzureStorageConnectionString(string loginName, string loginSecret)
        {
            return $"DefaultEndpointsProtocol=https;AccountName={loginName};AccountKey={loginSecret};EndpointSuffix=core.windows.net";
        }

        private async Task<IEnumerable<IFileInfo>> InternalGetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, bool namesOnly, CancellationToken cancellationToken)
        {
            var root = string.IsNullOrEmpty(rootBase) ? dataCollectionName : rootBase;
            var container = await GetContainerAsync(loginName, loginSecret, root, false, cancellationToken);

            var containerExists = await container.ExistsAsync();
            if (!containerExists)
            {
                return Enumerable.Empty<IFileInfo>();
            }

            BlobContinuationToken continuationToken = null;
            var prefix = string.IsNullOrEmpty(rootBase) ? null : (rootDir + dataCollectionName + "/");

            var prefixSections = string.IsNullOrEmpty(prefix) ? 0 : (prefix.TrimEnd('/').Split('/').Length);

            var resultList = new List<IFileInfo>();

            var blobListingDetails = namesOnly ? BlobListingDetails.None : BlobListingDetails.Metadata;

            while (true)
            {
                var result = await container.ListBlobsSegmentedAsync(prefix + fileNamePrefix, true, blobListingDetails, null, continuationToken, null, null, cancellationToken);

                var fileInfos = result.Results.OfType<CloudBlob>().Select(x => GetFileInfoAsync(x, prefixSections, namesOnly, cancellationToken).Result);
                resultList.AddRange(fileInfos);

                if (result.ContinuationToken == null)
                {
                    break;
                }

                continuationToken = result.ContinuationToken;
            }

            return resultList; 
        }

        private async Task<IFileInfo> GetFileInfoAsync(CloudBlob blob, int prefixSections, bool namesOnly, CancellationToken cancellationToken)
        {
            var name = string.Join("/", blob.Uri.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries).Skip(1 + prefixSections));

            if (namesOnly)
            {
                return await Task.FromResult(_fileInfoFactory(name, null, null, StorageType));
            }

            //await blob.FetchAttributesAsync();

            return await Task.FromResult(_fileInfoFactory(name, blob.Properties.Length, blob.Properties.ContentMD5, StorageType));
        }
    }
}
