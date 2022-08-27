using Abmes.DataCollector.Common.Storage;
using Amazon.S3;
using Amazon.S3.Model;

namespace Abmes.DataCollector.Common.Amazon.Storage
{
    public class AmazonCommonStorage : IAmazonCommonStorage
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IFileInfoDataFactory _fileInfoFactory;

        public string StorageType => "Amazon";

        public AmazonCommonStorage(
            IAmazonS3 amazonS3,
            IFileInfoDataFactory fileInfoFactory)
        {
            _amazonS3 = amazonS3;
            _fileInfoFactory = fileInfoFactory;
        }

        public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return
                (await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, true, cancellationToken))
                .Select(x => x.Name);
        }

        public async Task<IEnumerable<IFileInfoData>> GetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await InternalGetDataCollectionFileInfosAsync(loginName, loginSecret, rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
        }

        private async Task<IEnumerable<IFileInfoData>> InternalGetDataCollectionFileInfosAsync(string loginName, string loginSecret, string rootBase, string rootDir, string dataCollectionName, string fileNamePrefix, bool namesOnly, CancellationToken cancellationToken)
        {
            var prefix = rootDir + dataCollectionName + "/";

            var resultList = new List<IFileInfoData>();

            var request = new ListObjectsV2Request { BucketName = rootBase, Prefix = prefix + fileNamePrefix };

            while (true)
            {
                var response = await _amazonS3.ListObjectsV2Async(request);

                var fileInfos = response.S3Objects.Select(x => GetFileInfoAsync(x, prefix, namesOnly, cancellationToken).Result);
                resultList.AddRange(fileInfos);

                if (!response.IsTruncated)
                {
                    break;
                }

                request.ContinuationToken = response.NextContinuationToken;
            }

            return resultList;
        }

        private async Task<IFileInfoData> GetFileInfoAsync(S3Object s3Object, string prefix, bool namesOnly, CancellationToken cancellationToken)
        {
            var name = s3Object.Key.Substring(prefix.Length);

            if (namesOnly)
            {
                return _fileInfoFactory(name, null, null, null, StorageType);
            }

            var request = new GetObjectMetadataRequest { BucketName = s3Object.BucketName, Key = s3Object.Key };

            var response = await _amazonS3.GetObjectMetadataAsync(request, cancellationToken);

            var md5 = !string.IsNullOrEmpty(response.Headers.ContentMD5) ? response.Headers.ContentMD5 : response.Metadata["x-amz-meta-content-md5"];

            return _fileInfoFactory(name, response.Headers.ContentLength, md5, string.Join("/", name.Split('/').SkipLast(1)), StorageType);
        }
    }
}
