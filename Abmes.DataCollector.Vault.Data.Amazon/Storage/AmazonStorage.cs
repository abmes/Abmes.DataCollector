﻿using Abmes.DataCollector.Shared;
using Abmes.DataCollector.Shared.Data.Amazon.Storage;
using Abmes.DataCollector.Vault.Data.Configuration;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Amazon.S3;
using Amazon.S3.Model;

namespace Abmes.DataCollector.Vault.Data.Amazon.Storage;

public class AmazonStorage(
    StorageConfig storageConfig,
    TimeProvider timeProvider,
    IVaultAppSettings vaultAppSettings,
    IAmazonS3 amazonS3,
    IAmazonCommonStorage amazonCommonStorage)
    : IAmazonStorage
{
    public StorageConfig StorageConfig => storageConfig;

    public async Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = StorageConfig.RootBase(),
            Key = StorageConfig.RootDir('/', true) + dataCollectionName + "/" + fileName,
            Verb = HttpVerb.GET,
            Expires = timeProvider.GetUtcNow().Add(vaultAppSettings.DownloadUrlExpiry).DateTime
        };

        string result = amazonS3.GetPreSignedURL(request);

        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await amazonCommonStorage.GetDataCollectionFileNamesAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }

    public async Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken)
    {
        return await amazonCommonStorage.GetDataCollectionFileInfosAsync(StorageConfig.LoginName, StorageConfig.LoginSecret, StorageConfig.RootBase(), StorageConfig.RootDir('/', true), dataCollectionName, fileNamePrefix, cancellationToken);
    }
}
