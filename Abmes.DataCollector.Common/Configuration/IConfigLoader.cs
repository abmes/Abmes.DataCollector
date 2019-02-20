using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Configuration
{
    public interface IConfigLoader
    {
        bool CanLoadFrom(string storageType);
        Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
    }
}
