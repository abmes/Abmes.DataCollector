using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Storage
{
    public interface ICommonStorage
    {
        Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string loginName, string loginSecret, string root, string dataCollectionName, CancellationToken cancellationToken);
    }
}
