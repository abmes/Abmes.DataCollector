using Abmes.DataCollector.Collector.Common.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Amazon.Configuration
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly IAmazonAppSettings _commonAppSettings;
        private readonly IAmazonS3 _amazonS3;

        public ConfigProvider(
            IAmazonAppSettings commonAppSettings,
            IAmazonS3 amazonS3)
        {
            _commonAppSettings = commonAppSettings;
            _amazonS3 = amazonS3;
        }

        public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            var request = new GetObjectRequest { BucketName = _commonAppSettings.AmazonS3ConfigStorageBucketName, Key = configName };
            var response = _amazonS3.GetObjectAsync(request).Result;

            using (var reader = new System.IO.StreamReader(response.ResponseStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
