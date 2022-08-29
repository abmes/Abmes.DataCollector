using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Azure.Storage;
using Abmes.DataCollector.Utils;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using CommunityToolkit.HighPerformance;
using System.Text;

namespace Abmes.DataCollector.Collector.Azure.Destinations;

public class AzureDestination : IAzureDestination
{
    private readonly IAzureCommonStorage _azureCommonStorage;
    private readonly IHttpClientFactory _httpClientFactory;

    public DestinationConfig DestinationConfig { get; }

    public AzureDestination(
        DestinationConfig destinationConfig,
        IAzureCommonStorage azureCommonStorage,
        IHttpClientFactory httpClientFactory)
    {
        DestinationConfig = destinationConfig;
        _azureCommonStorage = azureCommonStorage;
        _httpClientFactory = httpClientFactory;
    }

    public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
    {
        var container = await GetContainerAsync(dataCollectionName, cancellationToken);

        await SmartCopyToBlobAsync(collectUrl, collectHeaders, container, GetBlobName(dataCollectionName, fileName), timeout, finishWait, cancellationToken);
    }

    private async Task<BlobContainerClient> GetContainerAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        var root = string.IsNullOrEmpty(DestinationConfig.Root) ? dataCollectionName : DestinationConfig.RootBase();
        return await _azureCommonStorage.GetContainerAsync(DestinationConfig.LoginName, DestinationConfig.LoginSecret, root, true, cancellationToken);
    }

    private string GetBlobName(string dataCollectionName, string fileName)
    {
        return string.IsNullOrEmpty(DestinationConfig.Root) ? fileName : (DestinationConfig.RootDir('/', true) + dataCollectionName + "/" + fileName);
    }

    private async Task SmartCopyToBlobAsync(string sourceUrl, IEnumerable<KeyValuePair<string, string>> sourceHeaders, BlobContainerClient container, string blobName, TimeSpan timeout, bool finishWait, CancellationToken cancellationToken)
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

    private async Task CopyFromUrlToBlob(string sourceUrl, IEnumerable<KeyValuePair<string, string>> sourceHeaders, BlobContainerClient container, string blobName, int bufferSize, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(sourceUrl, HttpMethod.Get, null, null, null, sourceHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await response.CheckSuccessAsync(cancellationToken);

        var sourceMD5 = response.ContentMD5();

        using var sourceStream = await response.Content.ReadAsStreamAsync();
        await CopyStreamToBlobAsync(sourceStream, container, blobName, bufferSize, sourceMD5, cancellationToken);
    }

    private async Task CopyStreamToBlobAsync(Stream sourceStream, BlobContainerClient container, string blobName, int bufferSize, string sourceMD5, CancellationToken cancellationToken)
    {
        var blob = container.GetBlockBlobClient(blobName);

        var blobHasher = CopyUtils.GetMD5Hasher();

        var blockIDs = new List<string>();
        var blockNumber = 0;

        await ParallelCopy.CopyAsync(
            (buffer, ct) => async () => await CopyUtils.ReadStreamMaxBufferAsync(buffer, sourceStream, ct),
            (buffer, ct) => async () =>
            {
                var blockId = GetBlockId(blockNumber);
                blockIDs.Add(blockId);

                var blockMD5Hash = CopyUtils.GetMD5Hash(buffer);
                CopyUtils.AppendMDHasherData(blobHasher, buffer);

                using var ms = buffer.AsStream();
                await blob.StageBlockAsync(blockId, ms, blockMD5Hash, null, null, ct);

                blockNumber++;
            },
            bufferSize,
            cancellationToken
        );

        var blobHash = CopyUtils.GetMD5Hash(blobHasher);

        if ((!string.IsNullOrEmpty(sourceMD5)) && (sourceMD5 != CopyUtils.GetMD5HashString(blobHash)))
        {
            throw new Exception("Invalid destination MD5");
        }

        var headers = new global::Azure.Storage.Blobs.Models.BlobHttpHeaders()
        {
            ContentHash = blobHash
        };

        await blob.CommitBlockListAsync(blockIDs, headers, null, null, null, cancellationToken);
    }

    private async Task AzureCopyToBlob(string sourceUrl, BlobContainerClient container, string blobName, bool finishWait, CancellationToken cancellationToken)
    {
        var blob = container.GetBlockBlobClient(blobName);

        var copyTaskId = await blob.StartCopyFromUriAsync(new Uri(sourceUrl), null, null, null, null, null, cancellationToken);

        if (finishWait)
        {
            await WaitCopyTaskAsync(container, blobName, cancellationToken);
        }
    }

    private async Task WaitCopyTaskAsync(BlobContainerClient container, string blobName, CancellationToken cancellationToken)
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);  // todo: config

            var blobs = container.GetBlobs(BlobTraits.CopyStatus, BlobStates.None, blobName, cancellationToken);

            var blob = blobs.FirstOrDefault();

            if (blob != null)
            {
                if ((blob.Properties.CopyStatus == CopyStatus.Failed) || (blob.Properties.CopyStatus == CopyStatus.Aborted))
                {
                    throw new Exception($"Copy of {blobName} failed: " + blob.Properties.CopyStatusDescription);
                }

                if (blob.Properties.CopyStatus == CopyStatus.Success)
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
        using var httpClient = _httpClientFactory.CreateClient();
        using var headRequest = new HttpRequestMessage(HttpMethod.Head, url);
        headers = headers?.Where(x => !string.Equals(x.Key, "Authorization", StringComparison.OrdinalIgnoreCase));  // anonymous access required for StartCopyAsync

        headRequest.Headers.AddValues(headers);

        using var headResult = await httpClient.SendAsync(headRequest, cancellationToken);

        if (!headResult.IsSuccessStatusCode)
        {
            return -1;
        }

        return headResult.Content.Headers.ContentLength ?? -1;
    }

    private async Task<long> GetGetContentLengthFromGetAsync(string url, IEnumerable<KeyValuePair<string, string>> headers, CancellationToken cancellationToken)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.AddValues(headers);

        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
        {
            return -1;
        }

        return response.Content.Headers.ContentLength ?? -1;
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var container = await GetContainerAsync(dataCollectionName, cancellationToken);
        var blob = container.GetBlobClient(GetBlobName(dataCollectionName, fileName));
        await blob.DeleteAsync(DeleteSnapshotsOption.None, null, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        return await _azureCommonStorage.GetDataCollectionFileNamesAsync(DestinationConfig.LoginName, DestinationConfig.LoginSecret, DestinationConfig.RootBase(), DestinationConfig.RootDir('/', true), dataCollectionName, null, cancellationToken);
    }

    public bool CanGarbageCollect()
    {
        return true;
    }

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string md5, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        var container = await GetContainerAsync(dataCollectionName, cancellationToken);
        var blob = container.GetBlobClient(GetBlobName(dataCollectionName, fileName));
        await blob.UploadAsync(content);
    }
}
