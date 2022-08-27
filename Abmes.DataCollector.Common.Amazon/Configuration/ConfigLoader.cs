using Abmes.DataCollector.Common.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

namespace Abmes.DataCollector.Common.Amazon.Configuration;

public class ConfigLoader : IConfigLoader
{
    private const string S3LocationPrefix = "s3://";
    private readonly IAmazonAppSettings _amazonAppSettings;
    private readonly IAmazonS3 _amazonS3;

    public ConfigLoader(
        IAmazonAppSettings amazonAppSettings,
        IAmazonS3 amazonS3)
    {
        _amazonAppSettings = amazonAppSettings;
        _amazonS3 = amazonS3;
    }

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
        var locationParts = location.Substring(S3LocationPrefix.Length).Split("/");

        var bucketName = locationParts.First();
        var root = string.Join("/", locationParts.Skip(1));

        if (!string.IsNullOrEmpty(root))
        {
            root = root + "/";
        }

        var request = new GetObjectRequest { BucketName = bucketName, Key = root + configName };
        var response = _amazonS3.GetObjectAsync(request).Result;

        using (var reader = new System.IO.StreamReader(response.ResponseStream, Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }

    public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        var location = S3LocationPrefix + _amazonAppSettings.AmazonS3ConfigStorageBucketName;
        return await GetConfigContentAsync(configName, location, cancellationToken);
    }
}
