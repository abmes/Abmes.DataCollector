using Abmes.DataCollector.Collector.Common.Identity;
using Abmes.DataCollector.Collector.Data.Configuration;
using Abmes.DataCollector.Common.Data.Amazon.Storage;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Utils.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CommunityToolkit.HighPerformance;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Data.Amazon.Destinations;

public class AmazonDestination(
    DestinationConfig destinationConfig,
    IAmazonS3 amazonS3,
    IAmazonCommonStorage amazonCommonStorage,
    IHttpClientFactory httpClientFactory,
    ILogger<AmazonDestination> logger) : IAmazonDestination
{
    public DestinationConfig DestinationConfig => destinationConfig;

    private static string? ContentMD5(HttpResponseMessage response)
    {
        return
            MD5Utils.GetMD5HashString(response.Content.Headers.ContentMD5)
            ??
            response.Headers.Where(x => x.Key.Equals("x-amz-meta-content-md5", StringComparison.InvariantCultureIgnoreCase)).Select(z => z.Value.FirstOrDefault()).FirstOrDefault();
    }

    public async Task CollectAsync(
        string collectUrl,
        IEnumerable<KeyValuePair<string, string>> collectHeaders,
        IdentityServiceClientInfo? collectIdentityServiceClientInfo,
        string dataCollectionName,
        string fileName,
        TimeSpan timeout,
        bool finishWait,
        CancellationToken cancellationToken)
    {
        using var httpClient = httpClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(collectUrl, HttpMethod.Get, collectUrl, null, null, collectHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var sourceMD5 = ContentMD5(response);

        using var sourceStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var key = dataCollectionName + '/' + fileName;

        await MultiPartUploadAsync(sourceStream, sourceMD5, DestinationConfig.RootBase(), DestinationConfig.RootDir('/', true) + key, cancellationToken);
    }

    private async Task MultiPartUploadAsync(Stream sourceStream, string? sourceMD5, string bucketName, string keyName, CancellationToken cancellationToken)
    {
        // Create list to store upload part responses.
        var uploadResponses = new List<UploadPartResponse>();

        // Setup information required to initiate the multipart upload.
        var initiateRequest = new InitiateMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = keyName
        };

        var validateMD5 = !string.IsNullOrEmpty(sourceMD5);

        if (validateMD5)
        {
            initiateRequest.Metadata.Add("Content-MD5", sourceMD5);
        }

        // Initiate the upload.
        var initResponse = await amazonS3.InitiateMultipartUploadAsync(initiateRequest, cancellationToken);
        try
        {
            using var blobHasher = validateMD5 ? MD5Utils.GetMD5Hasher() : null;

            var partSize = 10 * 1024 * 1024;  // todo: config
            var partNo = 1;

            await CopyUtils.ParallelCopyAsync(
                sourceStream.ReadAsync,
                async (buffer, ct) =>
                {
                    var blockMD5Hash = MD5Utils.GetMD5HashString(buffer);

                    if (validateMD5)
                    {
                        ArgumentNullException.ThrowIfNull(blobHasher);

                        MD5Utils.AppendMDHasherData(blobHasher, buffer);
                    }

                    using var ms = buffer.AsStream();
                    ms.Position = 0;

                    partNo++;

                    var uploadRequest = new UploadPartRequest
                    {
                        BucketName = bucketName,
                        Key = keyName,
                        UploadId = initResponse.UploadId,
                        PartNumber = partNo,
                        PartSize = buffer.Length,
                        InputStream = ms,
                        MD5Digest = blockMD5Hash,
                    };

                    // Upload a part and add the response to our list.
                    var uploadResponse = await amazonS3.UploadPartAsync(uploadRequest, cancellationToken);
                    uploadResponses.Add(uploadResponse);
                },
                partSize,
                cancellationToken
            );

            if (validateMD5)
            {
                ArgumentNullException.ThrowIfNull(blobHasher);

                var blobHash = MD5Utils.GetMD5HashString(blobHasher);

                if ((!string.IsNullOrEmpty(sourceMD5)) && (sourceMD5 != blobHash))
                {
                    throw new Exception("Invalid destination MD5");
                }
            }

            // Setup to complete the upload.
            var completeRequest = new CompleteMultipartUploadRequest
            {
                BucketName = bucketName,
                Key = keyName,
                UploadId = initResponse.UploadId
            };
            completeRequest.AddPartETags(uploadResponses);

            // Complete the upload.
            var completeUploadResponse = await amazonS3.CompleteMultipartUploadAsync(completeRequest, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An AmazonS3Exception was thrown: {message}", exception.Message);

            // Abort the upload.
            var abortMPURequest = new AbortMultipartUploadRequest
            {
                BucketName = bucketName,
                Key = keyName,
                UploadId = initResponse.UploadId
            };
            await amazonS3.AbortMultipartUploadAsync(abortMPURequest, cancellationToken);
        }
    }

    public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var key = DestinationConfig.RootDir('/', true) + dataCollectionName + '/' + fileName;

        var request = new DeleteObjectRequest { BucketName = DestinationConfig.RootBase(), Key = key };
        await amazonS3.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
    {
        return await amazonCommonStorage.GetDataCollectionFileNamesAsync(
            null, null, DestinationConfig.RootBase(), DestinationConfig.RootDir('/', true), dataCollectionName, null, cancellationToken);
    }

    public bool CanGarbageCollect()
    {
        return true;
    }

    public Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public async Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken)
    {
        var key = DestinationConfig.RootDir('/', true) + dataCollectionName + '/' + fileName;

        using var fileTransferUtility = new TransferUtility(amazonS3);

        await fileTransferUtility.UploadAsync(content, DestinationConfig.RootBase(), key, cancellationToken);
    }
}
