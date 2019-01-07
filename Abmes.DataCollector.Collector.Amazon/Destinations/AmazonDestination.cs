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

namespace Abmes.DataCollector.Collector.Amazon.Destinations
{
    public class AmazonDestination : IDestination
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IAmazonCommonStorage _amazonCommonStorage;

        public DestinationConfig DestinationConfig { get; set; }

        public AmazonDestination(
            IAmazonS3 amazonS3,
            IAmazonCommonStorage amazonCommonStorage)
        {
            _amazonS3 = amazonS3;
            _amazonCommonStorage = amazonCommonStorage;
        }

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.AddValues(collectHeaders);

                httpClient.Timeout = timeout;

                using (var response = await httpClient.GetAsync(collectUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    await response.CheckSuccessAsync();

                    using (var sourceStream = await response.Content.ReadAsStreamAsync())
                    {
                        var key = dataCollectionName + '/' + fileName;

                        await MultiPartUploadAsync(sourceStream, DestinationConfig.Root, key, cancellationToken);
                    }
                }
            }
        }

        private async Task MultiPartUploadAsync(Stream sourceStream, string bucketName, string keyName, CancellationToken cancellationToken)
        {
            // Create list to store upload part responses.
            var uploadResponses = new List<UploadPartResponse>();

            // Setup information required to initiate the multipart upload.
            var initiateRequest = new InitiateMultipartUploadRequest
            {
                BucketName = bucketName,
                Key = keyName
            };

            // Initiate the upload.
            var initResponse = await _amazonS3.InitiateMultipartUploadAsync(initiateRequest, cancellationToken);
            try
            {
                var partSize = 10 * 1024 * 1024;  // todo: config
                var partNo = 1;

                await CopyUtils.CopyAsync(
                    (buffer, ct) => CopyUtils.ReadStreamMaxBufferAsync(buffer, sourceStream, ct),
                    async (buffer, count, cancellationToken2) =>
                    {
                        using (var ms = new MemoryStream(buffer, 0, count))
                        {
                            ms.Position = 0;
                            File.WriteAllBytes(@"d:\test.zip", ms.ToArray());

                            ms.Position = 0;

                            partNo++;

                            var uploadRequest = new UploadPartRequest
                            {
                                BucketName = bucketName,
                                Key = keyName,
                                UploadId = initResponse.UploadId,
                                PartNumber = partNo,
                                PartSize = count,
                                InputStream = ms
                            };

                            // Upload a part and add the response to our list.
                            var uploadResponse = await _amazonS3.UploadPartAsync(uploadRequest, cancellationToken);
                            uploadResponses.Add(uploadResponse);
                        }
                    },
                    partSize,
                    cancellationToken
                );

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
            var request = new DeleteObjectRequest { BucketName = DestinationConfig.Root, Key = dataCollectionName + '/' + fileName };
            await _amazonS3.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken)
        {
            return await _amazonCommonStorage.GetDataCollectionFileNamesAsync(null, null, DestinationConfig.Root, dataCollectionName, cancellationToken);
        }
    }
}
