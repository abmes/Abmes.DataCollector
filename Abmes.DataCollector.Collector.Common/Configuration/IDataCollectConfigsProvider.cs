using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Configuration
{
    public interface IDataCollectConfigsProvider
    {
        Task<IEnumerable<DataCollectConfig>> GetDataCollectConfigsAsync(string configSetName, CancellationToken cancellationToken);
    }
}
