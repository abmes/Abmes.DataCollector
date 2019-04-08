using Abmes.DataCollector.Common.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Azure.Configuration
{
    public class ConfigLoader : IConfigLoader
    {
        private const string AzureConfigStorageConnectionStringName = "AzureConfigStorage";

        private readonly IConfiguration _configuration;
        private readonly IAzureAppSettings _azureAppSettings;

        public ConfigLoader(
            IConfiguration configuration,
            IAzureAppSettings azureAppSettings)
        {
            _configuration = configuration;
            _azureAppSettings = azureAppSettings;
        }

        public bool CanLoadFrom(string storageType)
        {
            return string.Equals(storageType, "Azure", StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            string connectionString = GetAzureStorageDbCollectConfigConnectionString();

            var account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);

            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(_azureAppSettings.AzureConfigStorageContainerName);

            var blob = container.GetBlobReference(configName);

            using (var contentStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(contentStream, null, null, null, cancellationToken);
                contentStream.Position = 0;

                using (var reader = new StreamReader(contentStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private string GetAzureStorageDbCollectConfigConnectionString()
        {
            var result = _configuration.GetConnectionString(AzureConfigStorageConnectionStringName);

            Contract.Assert(!string.IsNullOrEmpty(result));

            return result;
        }
    }
}
