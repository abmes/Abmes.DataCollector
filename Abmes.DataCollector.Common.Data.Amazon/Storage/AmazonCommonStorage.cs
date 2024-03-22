using Abmes.DataCollector.Common.Data.Amazon.Configuration;
using Abmes.DataCollector.Common.Storage;
using Amazon.S3;
using Amazon.S3.Model;

namespace Abmes.DataCollector.Common.Data.Amazon.Storage;

public class AmazonCommonStorage(
    IAmazonAppSettings amazonAppSettings,
    IAmazonS3 amazonS3) : IAmazonCommonStorage
{
    public string StorageType => "Amazon";

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(
        string? loginName,
        string? loginSecret,
        string rootBase,
        string rootDir,
        string dataCollectionName,
        string? fileNamePrefix,
        CancellationToken cancellationToken)
    {
        return
            (await InternalGetDataCollectionFileInfosAsync(rootBase, rootDir, dataCollectionName, fileNamePrefix, true, cancellationToken))
            .Select(x => x.Name);
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(
        string? loginName,
        string? loginSecret,
        string rootBase,
        string rootDir,
        string dataCollectionName,
        string? fileNamePrefix,
        CancellationToken cancellationToken)
    {
        return await InternalGetDataCollectionFileInfosAsync(rootBase, rootDir, dataCollectionName, fileNamePrefix, false, cancellationToken);
    }

    private async Task<IEnumerable<FileInfoData>> InternalGetDataCollectionFileInfosAsync(
        string rootBase,
        string rootDir,
        string dataCollectionName,
        string? fileNamePrefix,
        bool namesOnly,
        CancellationToken cancellationToken)
    {
        var prefix = rootDir + dataCollectionName + "/";

        var resultList = new List<FileInfoData>();

        var request = new ListObjectsV2Request { BucketName = rootBase, Prefix = prefix + fileNamePrefix };

        while (true)
        {
            var response = await amazonS3.ListObjectsV2Async(request, cancellationToken);

            var fileInfos = 
                    response.S3Objects
                    .AsParallel().WithDegreeOfParallelism(amazonAppSettings.AmazonS3ListParallelism ?? 1)
                    .Select(x => GetFileInfoAsync(x, prefix, namesOnly, cancellationToken).Result);

            resultList.AddRange(fileInfos);

            if (!response.IsTruncated)
            {
                break;
            }

            request.ContinuationToken = response.NextContinuationToken;
        }

        return resultList;
    }

    private async Task<FileInfoData> GetFileInfoAsync(S3Object s3Object, string prefix, bool namesOnly, CancellationToken cancellationToken)
    {
        var name = s3Object.Key[prefix.Length..];

        if (namesOnly)
        {
            return new FileInfoData(name, null, null, null, StorageType);
        }

        var request = new GetObjectMetadataRequest { BucketName = s3Object.BucketName, Key = s3Object.Key };

        var response = await amazonS3.GetObjectMetadataAsync(request, cancellationToken);

        var md5 = !string.IsNullOrEmpty(response.Headers.ContentMD5) ? response.Headers.ContentMD5 : response.Metadata["x-amz-meta-content-md5"];

        return new FileInfoData(name, response.Headers.ContentLength, md5, string.Join("/", name.Split('/').SkipLast(1)), StorageType);
    }
}
