using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Caching.Cache
{
    public interface IConfigFileCache
    {
        Task<string> GetConfigFileContentAsync(string fileName, IConfigProvider configFileProvider, CancellationToken cancellationToken);
    }
}
