using Abmes.DataCollector.Common.Azure.Configuration;
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

        public AzureCommonStorage(
            IAzureAppSettings commonAppSettings)
        {
            _commonAppSettings = commonAppSettings;
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

        private string GetAzureStorageConnectionString(string loginName, string loginSecret)
        {
            return $"DefaultEndpointsProtocol=https;AccountName={loginName};AccountKey={loginSecret};EndpointSuffix=core.windows.net";
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string root, string dataCollectionName, CancellationToken cancellationToken)
        {
            var container = await GetContainerAsync(loginName, loginSecret, root, false, cancellationToken);

            var containerExists = await container.ExistsAsync();
            if (!containerExists)
            {
                return Enumerable.Empty<string>();
            }

            BlobContinuationToken continuationToken = null;
            var prefix = dataCollectionName + "/";

            var resultList = new List<string>();

            while (true)
            {
                var result = await container.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.None, null, continuationToken, null, null, cancellationToken);

                foreach (var x in result.Results)
                {
                    var relativeFileName = string.Join("/", x.Uri.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries).Skip(2));
                    resultList.Add(relativeFileName);
                }

                if (result.ContinuationToken == null)
                    break;

                continuationToken = result.ContinuationToken;
            }

            return resultList;
        }
    }
}
