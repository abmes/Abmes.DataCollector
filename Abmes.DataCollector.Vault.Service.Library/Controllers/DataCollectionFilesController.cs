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

        // GET DataCollectionFiles/GetFiles?prefix=xyz
        [Route("GetFiles")]
        [HttpGet]
        public async Task<IEnumerable<IFileInfo>> GetFileInfosAsync([FromQuery] string prefix, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetFileInfosAsync(prefix, cancellationToken);
        }

        // GET DataCollectionFiles/GetFiles/latest
        [Route("GetFiles/latest")]
        [HttpGet]
        public async Task<IEnumerable<IFileInfo>> GetLatestFileInfosAsync(CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetLatestFileInfosAsync(cancellationToken);
        }

        // GET DataCollectionFiles/GetFiles?prefix=xyz
        [Route("GetFileNames")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetFileNamesAsync([FromQuery] string prefix, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetFileNamesAsync(prefix, cancellationToken);
        }

        // GET DataCollectionFiles/GetFiles/latest
        [Route("GetFileNames/latest")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetLatestFileNamesAsync(cancellationToken);
        }

        // GET DataCollectionFiles/GetDownloadUrl/some-file.zip
        [Route("GetDownloadUrl")]
        [HttpGet]
        public async Task<string> GetDownloadUrlAsync([FromQuery] string fileName, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetDownloadUrlAsync(fileName, cancellationToken);
        }

        // GET DataCollectionFiles/GetDownloadUrls?fileNamePrefix=xyz
        [Route("GetDownloadUrls")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetDownloadUrlsAsync([FromQuery] string fileNamePrefix, CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetDownloadUrlsAsync(fileNamePrefix, cancellationToken);
        }

        // GET DataCollectionFiles/GetDownloadUrl/latest
        [Route("GetLatestDownloadUrls")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetLatestDownloadUrls(CancellationToken cancellationToken)
        {
            return await _dataCollectionFiles.GetLatestDownloadUrlsAsync(cancellationToken);
        }

    }
}
