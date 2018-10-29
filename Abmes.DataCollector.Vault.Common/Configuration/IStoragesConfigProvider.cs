using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Configuration
{
    public interface IStoragesConfigProvider
    {
        Task<IEnumerable<StorageConfig>> GetStorageConfigsAsync(CancellationToken cancellationToken);
    }
}
