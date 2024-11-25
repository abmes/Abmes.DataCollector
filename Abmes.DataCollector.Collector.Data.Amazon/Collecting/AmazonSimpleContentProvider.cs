using Abmes.DataCollector.Collector.Services.Ports.Collecting;
using Amazon.S3;
using Amazon.S3.Model;

namespace Abmes.DataCollector.Collector.Data.Amazon.Collecting;

public class AmazonSimpleContentProvider(
    IAmazonS3 amazonS3) : ISimpleContentProvider
{
    private const string S3LocationPrefix = "s3://";

    public async Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken)
    {
        if (!uri.StartsWith(S3LocationPrefix))
        {
            return null;
        }

        var locationParts = uri[S3LocationPrefix.Length..].Split("/");
        var bucketName = locationParts.First();
        var key = string.Join("/", locationParts.Skip(1));

        var request = new GetObjectRequest { BucketName = bucketName, Key = key };
        var response = await amazonS3.GetObjectAsync(request, cancellationToken);

        using var ms = new MemoryStream();

        response.ResponseStream.CopyTo(ms);
        return ms.ToArray();
    }
}
