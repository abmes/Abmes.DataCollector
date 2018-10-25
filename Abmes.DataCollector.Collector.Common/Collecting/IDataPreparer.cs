using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IDataPreparer
    {
        Task PrepareDataAsync(DataCollectConfig DataCollectConfig, CancellationToken cancellationToken);
    }
}
