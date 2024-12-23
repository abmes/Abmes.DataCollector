﻿using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Shared.Data.Azure.Configuration;

public class ConfigLoader(
    IConfiguration configuration,
    IAzureAppSettings azureAppSettings)
    : IConfigLoader
{
    private const string AzureConfigStorageConnectionStringName = "AzureConfigStorage";

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

        var container = new BlobContainerClient(connectionString, azureAppSettings.AzureConfigStorageContainerName);

        var blob = container.GetBlobClient(configName);

        using var contentStream = new MemoryStream();
        await blob.DownloadToAsync(contentStream, cancellationToken);
        contentStream.Position = 0;

        using var reader = new StreamReader(contentStream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private string GetAzureStorageDbCollectConfigConnectionString()
    {
        var result = configuration.GetConnectionString(AzureConfigStorageConnectionStringName);

        ArgumentException.ThrowIfNullOrEmpty(result);

        return result;
    }
}
