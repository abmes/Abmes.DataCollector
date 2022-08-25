using Abmes.DataCollector.Common.Storage;
using Abmes.DataCollector.Vault.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Service.Controllers
{
    [Authorize(Policy = "UserAllowedDataCollection")]
    [Route("[controller]")]
    public class DataCollectionFilesController : Controller
    {
        private readonly IDataCollectionFiles _dataCollectionFiles;

        public DataCollectionFilesController(IDataCollectionFiles dataCollectionFiles)
        {
            _dataCollectionFiles = dataCollectionFiles;
        }

        private TimeSpan? ParseTimeSpan(string value)
        {
            return string.IsNullOrEmpty(value) ? (TimeSpan?)null : TimeSpan.Parse(value);
        }

        // GET DataCollectionFiles/GetFiles?prefix=xyz&maxAge=0:2:0
        [Route("GetFiles")]
        [HttpGet]
        public async Task<IEnumerable<IFileInfoData>> GetFileInfosAsync([FromQuery] string prefix, [FromQuery] string maxAge, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetFileInfosAsync(prefix, ParseTimeSpan(maxAge), cancellationToken);
        }

        // GET DataCollectionFiles/GetFiles/latest
        [Route("GetFiles/latest")]
        [HttpGet]
        public async Task<IEnumerable<IFileInfoData>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetLatestFileInfosAsync(cancellationToken);
        }

        // GET DataCollectionFiles/GetFiles?prefix=xyz&maxAge=0:2:0
        [Route("GetFileNames")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetFileNamesAsync([FromQuery] string prefix, [FromQuery] string maxAge, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetFileNamesAsync(prefix, ParseTimeSpan(maxAge), cancellationToken);
        }

        // GET DataCollectionFiles/GetFiles/latest
        [Route("GetFileNames/latest")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetLatestFileNamesAsync(cancellationToken);
        }

        // GET DataCollectionFiles/GetDownloadUrl?fileName=some-file.zip&storageType=Amazon
        [Route("GetDownloadUrl")]
        [HttpGet]
        public async Task<string> GetDownloadUrlAsync([FromQuery] string fileName, [FromQuery] string storageType, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetDownloadUrlAsync(fileName, storageType, cancellationToken);
        }

        // GET DataCollectionFiles/GetDownloadUrls?fileNamePrefix=xyz&storageType=Amazon
        [Route("GetDownloadUrls")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetDownloadUrlsAsync([FromQuery] string fileNamePrefix, [FromQuery] string storageType, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetDownloadUrlsAsync(fileNamePrefix, storageType, cancellationToken);
        }

        // GET DataCollectionFiles/GetDownloadUrl/latest?storageType=Amazon
        [Route("GetLatestDownloadUrls")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetLatestDownloadUrls([FromQuery] string storageType, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetLatestDownloadUrlsAsync(storageType, cancellationToken);
        }
    }
}
