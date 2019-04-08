using Abmes.DataCollector.Common.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Amazon.Configuration
{
    public class ConfigLoader : IConfigLoader
    {
        private readonly IAmazonAppSettings _amazonAppSettings;
        private readonly IAmazonS3 _amazonS3;

        public ConfigLoader(
            IAmazonAppSettings amazonAppSettings,
            IAmazonS3 amazonS3)
        {
            _amazonAppSettings = amazonAppSettings;
            _amazonS3 = amazonS3;
        }

        public bool CanLoadFrom(string storageType)
        {
            return string.Equals(storageType, "Amazon", StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest { BucketName = _amazonAppSettings.AmazonS3ConfigStorageBucketName, Key = configName };
            var response = _amazonS3.GetObjectAsync(request).Result;

            using (var reader = new System.IO.StreamReader(response.ResponseStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
