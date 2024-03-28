using Abmes.DataCollector.Common;
using Abmes.DataCollector.Vault.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abmes.DataCollector.Vault.Web.Controllers;

[Authorize(Policy = "UserAllowedDataCollection")]
[Route("[controller]")]
public class DataCollectionFilesController(
    IDataCollectionFiles dataCollectionFiles) : ControllerBase
{
    private static TimeSpan? ParseTimeSpan(string? value)
    {
        return string.IsNullOrEmpty(value) ? null : TimeSpan.Parse(value);
    }

    // GET DataCollectionFiles/GetFiles?prefix=xyz&maxAge=0:2:0
    [Route("GetFiles")]
    [HttpGet]
    public async Task<IEnumerable<FileInfoData>> GetFileInfosAsync([FromQuery] string? prefix, [FromQuery] string? maxAge, CancellationToken cancellationToken)
    {
        return await dataCollectionFiles.GetFileInfosAsync(prefix, ParseTimeSpan(maxAge), cancellationToken);
    }

    // GET DataCollectionFiles/GetFiles/latest
    [Route("GetFiles/latest")]
    [HttpGet]
    public async Task<IEnumerable<FileInfoData>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
    {
        return await dataCollectionFiles.GetLatestFileInfosAsync(cancellationToken);
    }

    // GET DataCollectionFiles/GetFiles?prefix=xyz&maxAge=0:2:0
    [Route("GetFileNames")]
    [HttpGet]
    public async Task<IEnumerable<string>> GetFileNamesAsync([FromQuery] string? prefix, [FromQuery] string? maxAge, CancellationToken cancellationToken)
    {
        return await dataCollectionFiles.GetFileNamesAsync(prefix, ParseTimeSpan(maxAge), cancellationToken);
    }

    // GET DataCollectionFiles/GetFiles/latest
    [Route("GetFileNames/latest")]
    [HttpGet]
    public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
    {
        return await dataCollectionFiles.GetLatestFileNamesAsync(cancellationToken);
    }

    // GET DataCollectionFiles/GetDownloadUrl?fileName=some-file.zip&storageType=Amazon
    [Route("GetDownloadUrl")]
    [HttpGet]
    public async Task<string> GetDownloadUrlAsync([FromQuery] string? fileName, [FromQuery] string? storageType, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        return await dataCollectionFiles.GetDownloadUrlAsync(fileName, storageType, cancellationToken);
    }

    // GET DataCollectionFiles/GetDownloadUrls?fileNamePrefix=xyz&storageType=Amazon
    [Route("GetDownloadUrls")]
    [HttpGet]
    public async Task<IEnumerable<string>> GetDownloadUrlsAsync([FromQuery] string? fileNamePrefix, [FromQuery] string? storageType, CancellationToken cancellationToken)
    {
        return await dataCollectionFiles.GetDownloadUrlsAsync(fileNamePrefix, storageType, cancellationToken);
    }

    // GET DataCollectionFiles/GetDownloadUrl/latest?storageType=Amazon
    [Route("GetLatestDownloadUrls")]
    [HttpGet]
    public async Task<IEnumerable<string>> GetLatestDownloadUrls([FromQuery] string? storageType, CancellationToken cancellationToken)
    {
        return await dataCollectionFiles.GetLatestDownloadUrlsAsync(storageType, cancellationToken);
    }
}
