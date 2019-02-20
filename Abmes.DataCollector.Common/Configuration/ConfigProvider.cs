using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Configuration
{
    public class ConfigProvider : IConfigProvider
    {
        private readonly IEnumerable<IConfigLoader> _configLoaders;

        public ConfigProvider(
            IEnumerable<IConfigLoader> configLoaders)
        {
            _configLoaders = configLoaders;
        }

        public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            var configStorageType = "Amazon";
            var configLoader = _configLoaders.Where(x => x.CanLoadFrom(configStorageType)).Single();
            return await configLoader.GetConfigContentAsync(configName, cancellationToken);
        }
    }
}
