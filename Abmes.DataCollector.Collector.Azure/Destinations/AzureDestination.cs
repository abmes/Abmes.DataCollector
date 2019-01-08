using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Common.Azure.Storage;
using Abmes.DataCollector.Utils;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Azure.Destinations
{
    public class AzureDestination : IDestination
    {
        private readonly IAzureCommonStorage _azureCommonStorage;

        public DestinationConfig DestinationConfig { get; set; }

        public AzureDestination(
            IAzureCommonStorage azureCommonStorage)
        {
            _azureCommonStorage = azureCommonStorage;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string databaseName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            var container = await GetContainerAsync(cancellationToken);

            await SmartCopyToBlobAsync(collectUrl, collectHeaders, container, GetBlobName(databaseName, fileName), timeout, finishWait, cancellationToken);
        }

        private async Task<CloudBlobContainer> GetContainerAsync(CancellationToken cancellationToken)
        {
            return await _azureCommonStorage.GetContainerAsync(DestinationConfig.LoginName, DestinationConfig.LoginSecret, DestinationConfig.Root, true, cancellationToken);
        }

        private static string GetBlobName(string databaseName, string fileName)
        {
            return databaseName + "/" + fileName;
        }

        private async Task SmartCopyToBlobAsync(string sourceUrl, IEnumerable<KeyValuePair<string, string>> sourceHeaders, CloudBlobContainer container, string blobName, TimeSpan timeout, bool finishWait, CancellationToken cancellationToken)
        {
            if (await GetContentLengthHeaderAsync(sourceUrl, sourceHeaders, cancellationToken) > 0)
            {
                await AzureCopyToBlob(sourceUrl, container, blobName, finishWait, cancellationToken);
            }
            else
            {
                await CopyFromUrlToBlob(sourceUrl, sourceHeaders, container, blobName, 1 * 1024 * 1024, timeout, cancellationToken);
            }
        }

        private async Task CopyFromUrlToBlob(string sourceUrl, IEnumerable<KeyValuePair<string, string>> sourceHeaders, CloudBlobContainer container, string blobName, int bufferSize, TimeSpan timeout, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.AddValues(sourceHeaders);

                httpClient.Timeout = timeout;

                using (var response = await httpClient.GetAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    await response.CheckSuccessAsync();

                    using (var sourceStream = await response.Content.ReadAsStreamAsync())
                    {
                        var blob = container.GetBlockBlobReference(blobName);

                        var blobHasher = CopyUtils.GetMD5Hasher();

                        var blockIDs = new List<string>();
                        var blockNumber = 0;

                        await CopyUtils.CopyAsync(
                                (buffer, ct) => CopyUtils.ReadStreamMaxBufferAsync(buffer, sourceStream, ct),
                                async (buffer, count, cancellationToken2) =>
                                {
                                    var blockId = GetBlockId(blockNumber);
                                    blockIDs.Add(blockId);

                                    var blockMD5Hash = CopyUtils.GetMD5Hash(buffer, 0, count);
                                    CopyUtils.AppendMDHasherData(blobHasher, buffer, 0, count);

                                    using (var ms = new MemoryStream(buffer, 0, count))
                                    {
                                        await blob.PutBlockAsync(blockId, ms, blockMD5Hash, null, null, null, cancellationToken);
                                    }

                                    blockNumber++;
                                },
                                bufferSize,
                                cancellationToken
                            );

                        blob.Properties.ContentMD5 = CopyUtils.GetMD5Hash(blobHasher);
                        await blob.PutBlockListAsync(blockIDs, null, null, null, cancellationToken);
                    }
                }
            }
        }

        private async Task AzureCopyToBlob(string sourceUrl, CloudBlobContainer container, string blobName, bool finishWait, CancellationToken cancellationToken)
        {
            var blob = container.GetBlobReference(blobName);
            var copyTaskId = await blob.StartCopyAsync(new Uri(sourceUrl), null, null, null, null, cancellationToken);

            if (finishWait)
            {
                await WaitCopyTaskAsync(container, blobName, cancellationToken);
            }
        }

        private async Task WaitCopyTaskAsync(CloudBlobContainer container, string blobName, CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);

                var blobs = container.ListBlobsSegmentedAsync(blobName, true, BlobListingDetails.Copy, 1, null, null, null, cancellationToken);

                var blob = blobs.Result.Results.FirstOrDefault() as CloudBlob;

                if (blob != null)
                {
                    if ((blob.CopyState.Status == CopyStatus.Failed) || (blob.CopyState.Status == CopyStatus.Aborted))
                    {
                        throw new Exception($"Copy of {blobName} failed: " + blob.CopyState.StatusDescription);
                    }

                    if (blob.CopyState.Status == CopyStatus.Success)
                    {
                        break;
                    }
                }
            }
        }

        private static string GetBlockId(int blockNumber)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("BlockId{0}", blockNumber.ToString("0000000"))));
        }

        private async Task<long> GetContentLengthHeaderAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                using (var headRequest = new HttpRequestMessage(HttpMethod.Head, url))
                {
                    headers = headers?.Where(x => !string.Equals(x.Key, "Authorization", StringComparison.OrdinalIgnoreCase));  // anonymous access required for StartCopyAsync

                    headRequest.Headers.AddValues(headers);

                    var headResult = await httpClient.SendAsync(headRequest, cancellationToken);

                    if (!headResult.IsSuccessStatusCode)
                    {
                        return -1;
                    }

                    return headResult.Content.Headers.ContentLength ?? -1;
                }
            }
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var container = await GetContainerAsync(cancellationToken);
            var blob = container.GetBlobReference(GetBlobName(dataCollectionName, fileName));
            await blob.DeleteAsync(DeleteSnapshotsOption.None, null, null, null, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _azureCommonStorage.GetDataCollectionFileNamesAsync(DestinationConfig.LoginName, DestinationConfig.LoginSecret, DestinationConfig.Root, dataCollectionName, cancellationToken);
        }
    }
}
