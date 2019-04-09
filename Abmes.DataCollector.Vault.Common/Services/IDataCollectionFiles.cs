using Abmes.DataCollector.Common.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Services
{
    public interface IDataCollectionFiles
    {
        Task<IEnumerable<IFileInfo>> GetFileInfosAsync(string prefix, CancellationToken cancellationToken);
        Task<IEnumerable<IFileInfo>> GetLatestFileInfosAsync(CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetFileNamesAsync(string prefix, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken);
        Task<string> GetDownloadUrlAsync(string fileName, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetDownloadUrlsAsync(string fileNamePrefix, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetLatestDownloadUrlsAsync(CancellationToken cancellationToken);
    }
}
