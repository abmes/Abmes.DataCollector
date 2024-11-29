using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

namespace Abmes.DataCollector.Shared.Data.Amazon.Configuration;

public class ConfigLoader(
    IAmazonAppSettings amazonAppSettings,
    IAmazonS3 amazonS3)
    : IConfigLoader
{
    private const string S3LocationPrefix = "s3://";

    public bool CanLoadFromStorage(string storageType)
    {
        return string.Equals(storageType, "Amazon", StringComparison.InvariantCultureIgnoreCase);
    }

    public bool CanLoadFromLocation(string location)
    {
        return 
            location.StartsWith(S3LocationPrefix, StringComparison.InvariantCultureIgnoreCase) && 
            !location.Equals(S3LocationPrefix, StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken)
    {
        var locationParts = location[S3LocationPrefix.Length..].Split("/");

        var bucketName = locationParts.First();
        var root = string.Join("/", locationParts.Skip(1));

        if (!string.IsNullOrEmpty(root))
        {
            root += "/";
        }

        var request = new GetObjectRequest { BucketName = bucketName, Key = root + configName };
        var response = await amazonS3.GetObjectAsync(request, cancellationToken);

        using var reader = new StreamReader(response.ResponseStream, Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        var location = S3LocationPrefix + amazonAppSettings.AmazonS3ConfigStorageBucketName;
        return await GetConfigContentAsync(configName, location, cancellationToken);
    }
}
