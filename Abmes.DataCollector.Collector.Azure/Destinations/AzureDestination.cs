﻿using Abmes.DataCollector.Collector.Common.Configuration;
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
    public class AzureDestination : IAzureDestination
    {
        private readonly IAzureCommonStorage _azureCommonStorage;

        public DestinationConfig DestinationConfig { get; }

        public AzureDestination(
            DestinationConfig destinationConfig,
            IAzureCommonStorage azureCommonStorage)
        {
            DestinationConfig = destinationConfig;
            _azureCommonStorage = azureCommonStorage;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            var container = await GetContainerAsync(dataCollectionName, cancellationToken);

            await SmartCopyToBlobAsync(collectUrl, collectHeaders, container, GetBlobName(dataCollectionName, fileName), timeout, finishWait, cancellationToken);
        }

        private async Task<CloudBlobContainer> GetContainerAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            var root = string.IsNullOrEmpty(DestinationConfig.Root) ? dataCollectionName : DestinationConfig.RootBase();
            return await _azureCommonStorage.GetContainerAsync(DestinationConfig.LoginName, DestinationConfig.LoginSecret, root, true, cancellationToken);
        }

        private string GetBlobName(string dataCollectionName, string fileName)
        {
            return string.IsNullOrEmpty(DestinationConfig.Root) ? fileName : (DestinationConfig.RootDir('/', true) + dataCollectionName + "/" + fileName);
        }

        private async Task SmartCopyToBlobAsync(string sourceUrl, IEnumerable<KeyValuePair<string, string>> sourceHeaders, CloudBlobContainer container, string blobName, TimeSpan timeout, bool finishWait, CancellationToken cancellationToken)
        {
            //if (await GetContentLengthAsync(sourceUrl, sourceHeaders, cancellationToken) > 0)
            //{
            //    await AzureCopyToBlob(sourceUrl, container, blobName, finishWait, cancellationToken);
            //}
            //else
            {
                await CopyFromUrlToBlob(sourceUrl, sourceHeaders, container, blobName, 1 * 1024 * 1024, timeout, cancellationToken);
            }
        }

        private async Task CopyFromUrlToBlob(string sourceUrl, IEnumerable<KeyValuePair<string, string>> sourceHeaders, CloudBlobContainer container, string blobName, int bufferSize, TimeSpan timeout, CancellationToken cancellationToken)
        {
            using (var response = await HttpUtils.SendAsync(sourceUrl, HttpMethod.Get, null, sourceHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                await response.CheckSuccessAsync();

                var sourceMD5 = response.ContentMD5();

                using (var sourceStream = await response.Content.ReadAsStreamAsync())
                {
                    await CopyStreamToBlobAsync(sourceStream, container, blobName, bufferSize, sourceMD5, cancellationToken);
                }
            }
        }

        private static async Task CopyStreamToBlobAsync(Stream sourceStream, CloudBlobContainer container, string blobName, int bufferSize, string sourceMD5, CancellationToken cancellationToken)
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

            var blobHash = CopyUtils.GetMD5Hash(blobHasher);

            if ((!string.IsNullOrEmpty(sourceMD5)) && (sourceMD5 != blobHash))
            {
                throw new Exception("Invalid destination MD5");
            }

            blob.Properties.ContentMD5 = blobHash;
            await blob.PutBlockListAsync(blockIDs, null, null, null, cancellationToken);
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
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);  // todo: config

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

        private async Task<long> GetContentLengthAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            var headContentLength = await GetContentLengthFromHeadAsync(url, headers, cancellationToken);

            if (headContentLength == -1)
            {
                return await GetGetContentLengthFromGetAsync(url, headers, cancellationToken);
            }
            else
            {
                return headContentLength;
            }
        }

        private async Task<long> GetContentLengthFromHeadAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
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

        private async Task<long> GetGetContentLengthFromGetAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.AddValues(headers);

                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return -1;
                    }

                    return response.Content.Headers.ContentLength ?? -1;
                }
            }
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var container = await GetContainerAsync(dataCollectionName, cancellationToken);
            var blob = container.GetBlobReference(GetBlobName(dataCollectionName, fileName));
            await blob.DeleteAsync(DeleteSnapshotsOption.None, null, null, null, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _azureCommonStorage.GetDataCollectionFileNamesAsync(DestinationConfig.LoginName, DestinationConfig.LoginSecret, DestinationConfig.RootBase(), DestinationConfig.RootDir('/', true), dataCollectionName, null, cancellationToken);
        }

        public bool CanGarbageCollect()
        {
            return true;
        }
    }
}
