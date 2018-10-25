using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Services
{
    public interface IDataCollectionFiles
    {
        Task<IEnumerable<string>> GetFileNamesAsync(CancellationToken cancellationToken);
        Task<List<string>> GetLatestFileNamesAsync(CancellationToken cancellationToken);
        Task<string> GetDownloadUrlAsync(string fileName, CancellationToken cancellationToken);
        Task<List<string>> GetLatestDownloadUrlsAsync(CancellationToken cancellationToken);
    }
}
