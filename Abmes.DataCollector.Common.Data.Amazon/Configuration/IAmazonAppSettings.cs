namespace Abmes.DataCollector.Common.Data.Amazon.Configuration;

public interface IAmazonAppSettings
{
    string AmazonS3ConfigStorageBucketName { get; }
    int? AmazonS3ListParallelism { get; }
}
