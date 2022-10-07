﻿using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Vault.Services;

public interface IDataCollectionFiles
{
    Task<IEnumerable<FileInfoData>> GetFileInfosAsync(string? prefix, TimeSpan? maxAge, CancellationToken cancellationToken);
    Task<IEnumerable<FileInfoData>> GetLatestFileInfosAsync(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetFileNamesAsync(string? prefix, TimeSpan? maxAge, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken);
    Task<string> GetDownloadUrlAsync(string fileName, string? storageType, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetDownloadUrlsAsync(string? fileNamePrefix, string? storageType, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(string? storageType, CancellationToken cancellationToken);
}
