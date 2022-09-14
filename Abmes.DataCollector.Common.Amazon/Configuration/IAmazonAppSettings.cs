namespace Abmes.DataCollector.Common.Amazon.Configuration;

public interface IAmazonAppSettings
{
    string AmazonS3ConfigStorageBucketName { get; }
    int? AmazonS3ListParallelism { get; }
}
