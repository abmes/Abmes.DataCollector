using Abmes.DataCollector.Vault.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Vault.Storage
{
    public interface IStoragesProvider
    {
        Task<IEnumerable<IStorage>> GetStoragesAsync(CancellationToken cancellationToken);
    }
}
