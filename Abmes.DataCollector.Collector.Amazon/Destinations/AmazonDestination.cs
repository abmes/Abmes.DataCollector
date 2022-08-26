using Abmes.DataCollector.Collector.Common.Destinations;
using Abmes.DataCollector.Utils;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Common.Amazon.Storage;
using Abmes.DataCollector.Collector.Common.Configuration;
using System.IO;
using System.Linq;
using CommunityToolkit.HighPerformance;

namespace Abmes.DataCollector.Collector.Amazon.Destinations
{
    public class AmazonDestination : IAmazonDestination
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IAmazonCommonStorage _amazonCommonStorage;
        private readonly IHttpClientFactory _httpClientFactory;

        public DestinationConfig DestinationConfig { get; }

        public AmazonDestination(
            DestinationConfig destinationConfig,
            IAmazonS3 amazonS3,
            IAmazonCommonStorage amazonCommonStorage,
            IHttpClientFactory httpClientFactory)
        {
            DestinationConfig = destinationConfig;
            _amazonS3 = amazonS3;
            _amazonCommonStorage = amazonCommonStorage;
            _httpClientFactory = httpClientFactory;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IIdentityServiceClientInfo collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            using (var response = await httpClient.SendAsync(collectUrl, HttpMethod.Get, collectUrl, null, null, collectHeaders, null, timeout, null, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var sourceMD5 = response.ContentMD5();

                using (var sourceStream = await response.Content.ReadAsStreamAsync())
                {
                    var key = dataCollectionName + '/' + fileName;

                    await MultiPartUploadAsync(sourceStream, sourceMD5, DestinationConfig.RootBase(), DestinationConfig.RootDir('/', true) + key, cancellationToken);
                }
            }
        }

        private async Task MultiPartUploadAsync(Stream sourceStream, string sourceMD5, string bucketName, string keyName, CancellationToken cancellationToken)
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
            var initResponse = await _amazonS3.InitiateMultipartUploadAsync(initiateRequest, cancellationToken);
            try
            {
                var blobHasher = validateMD5 ? CopyUtils.GetMD5Hasher() : null;

                var partSize = 10 * 1024 * 1024;  // todo: config
                var partNo = 1;

                await ParallelCopy.CopyAsync(
                    (buffer, ct) => async () => await CopyUtils.ReadStreamMaxBufferAsync(buffer, sourceStream, ct),
                    (buffer, ct) => async () =>
                    {
                        var blockMD5Hash = CopyUtils.GetMD5HashString(buffer);

                        if (validateMD5)
                        {
                            CopyUtils.AppendMDHasherData(blobHasher, buffer);
                        }

                        using (var ms = buffer.AsStream())
                        {
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
                            var uploadResponse = await _amazonS3.UploadPartAsync(uploadRequest, cancellationToken);
                            uploadResponses.Add(uploadResponse);
                        }
                    },
                    partSize,
                    cancellationToken
                );

                if (validateMD5)
                {
                    var blobHash = CopyUtils.GetMD5HashString(blobHasher);

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
                var completeUploadResponse = await _amazonS3.CompleteMultipartUploadAsync(completeRequest, cancellationToken);
            }
            catch (Exception exception)
            {
                Console.WriteLine("An AmazonS3Exception was thrown: { 0}", exception.Message);

                // Abort the upload.
                var abortMPURequest = new AbortMultipartUploadRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    UploadId = initResponse.UploadId
                };
                await _amazonS3.AbortMultipartUploadAsync(abortMPURequest, cancellationToken);
            }
        }

        public async Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
        {
            var key = DestinationConfig.RootDir('/', true) + dataCollectionName + '/' + fileName;

            var request = new DeleteObjectRequest { BucketName = DestinationConfig.RootBase(), Key = key };
            await _amazonS3.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _amazonCommonStorage.GetDataCollectionFileNamesAsync(null, null, DestinationConfig.RootBase(), DestinationConfig.RootDir('/', true), dataCollectionName, null, cancellationToken);
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
            var key = DestinationConfig.RootDir('/', true) + dataCollectionName + '/' + fileName;

            using (var fileTransferUtility = new TransferUtility(_amazonS3))
            {
                fileTransferUtility.Upload(content, DestinationConfig.RootBase(), key);
            }

            await Task.CompletedTask;
        }
    }
}
