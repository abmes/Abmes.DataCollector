using Abmes.DataCollector.Common.Configuration;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.Contracts;

namespace Abmes.DataCollector.Common.Azure.Configuration;

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

    public bool CanLoadFromStorage(string storageType)
    {
        return string.Equals(storageType, "Azure", StringComparison.InvariantCultureIgnoreCase);
    }

    public bool CanLoadFromLocation(string location)
    {
        return false;
    }

    public Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
    {
        string connectionString = GetAzureStorageDbCollectConfigConnectionString();

        var container = new BlobContainerClient(connectionString, _azureAppSettings.AzureConfigStorageContainerName);

        var blob = container.GetBlobClient(configName);

        using (var contentStream = new MemoryStream())
        {
            await blob.DownloadToAsync(contentStream, cancellationToken);
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
