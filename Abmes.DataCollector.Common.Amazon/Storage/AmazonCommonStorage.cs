using Abmes.DataCollector.Common.Storage;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Amazon.Storage
{
    public class AmazonCommonStorage : IAmazonCommonStorage
    {
        private readonly IAmazonS3 _amazonS3;

        public AmazonCommonStorage(
            IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, CancellationToken cancellationToken)
        {
            var prefix = rootDir + dataCollectionName + "/";

            var resultList = new List<string>();

            var request = new ListObjectsV2Request { BucketName = rootBase, Prefix = prefix };

            while (true)
            {
                var response = await _amazonS3.ListObjectsV2Async(request);

                var relativeFileNames = response.S3Objects.Select(x => x.Key.Substring(prefix.Length));
                resultList.AddRange(relativeFileNames);

                if (!response.IsTruncated)
                    break;

                request.ContinuationToken = response.NextContinuationToken;
            }

            return resultList;
        }
    }
}
