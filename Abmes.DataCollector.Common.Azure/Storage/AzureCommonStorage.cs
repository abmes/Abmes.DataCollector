using Abmes.DataCollector.Common.Azure.Configuration;
using Abmes.DataCollector.Common.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

        public async Task<BlobContainerClient> GetContainerAsync(string loginName, string loginSecret, string root, bool createIfNotExists, CancellationToken cancellationToken)
        {
            var connectionString = GetConnectionString(loginName, loginSecret);

            var container = new BlobContainerClient(connectionString, root);

            if (createIfNotExists)
            {
                await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
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

        public string GetConnectionString(string loginName, string loginSecret)
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

            var prefix = string.IsNullOrEmpty(rootBase) ? null : (rootDir + dataCollectionName + "/");

            var prefixSections = string.IsNullOrEmpty(prefix) ? 0 : (prefix.TrimEnd('/').Split('/').Length);

            var blobTraits = namesOnly ? BlobTraits.None : BlobTraits.Metadata;

            var blobs = container.GetBlobs(blobTraits, BlobStates.None, prefix + fileNamePrefix, cancellationToken);

            return blobs.Select(x => GetFileInfoAsync(x, prefixSections, namesOnly, cancellationToken).Result).ToList();
        }

        private async Task<IFileInfo> GetFileInfoAsync(BlobItem blob, int prefixSections, bool namesOnly, CancellationToken cancellationToken)
        {
            var name = string.Join("/", blob.Name.Split("/", StringSplitOptions.RemoveEmptyEntries).Skip(prefixSections));

            if (namesOnly)
            {
                return await Task.FromResult(_fileInfoFactory(name, null, null, null, StorageType));
            }

            return await Task.FromResult(_fileInfoFactory(name, blob.Properties.ContentLength, Convert.ToBase64String(blob.Properties.ContentHash), string.Join("/", name.Split('/').SkipLast(1)), StorageType));
        }
    }
}
