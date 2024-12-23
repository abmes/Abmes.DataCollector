﻿using Abmes.DataCollector.Shared;
using Abmes.DataCollector.Vault.Services.Ports.Configuration;

namespace Abmes.DataCollector.Vault.Services.Ports.Storage;

public interface IStorage
{
    StorageConfig StorageConfig { get; }
    Task<IEnumerable<FileInfoData>> GetDataCollectionFileInfosAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, string? fileNamePrefix, CancellationToken cancellationToken);
    Task<string> GetDataCollectionFileDownloadUrlAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken);
}
