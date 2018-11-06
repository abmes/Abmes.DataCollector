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

        public async Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, CancellationToken cancellationToken)
        {
            using (var transferUtility = new TransferUtility(_amazonS3))
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
                            await transferUtility.UploadAsync(sourceStream, DestinationConfig.Root, key, cancellationToken);
                        }
                    }
                }
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
