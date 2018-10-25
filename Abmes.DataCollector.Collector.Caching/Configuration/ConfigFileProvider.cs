using Abmes.DataCollector.Collector.Caching.Cache;
using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Caching.Configuration
{
    public class ConfigFileProvider : IConfigProvider
    {
        private readonly IConfigProvider _configFileProvider;
        private readonly IConfigFileCache _configFileCache;

        public ConfigFileProvider(
            IConfigProvider configFileProvider,
            IConfigFileCache configFileCache)
        {
            _configFileProvider = configFileProvider;
            _configFileCache = configFileCache;
        }

        public async Task<string> GetConfigContentAsync(string fileName, CancellationToken cancellationToken)
        {
            return await _configFileCache.GetConfigFileContentAsync(fileName, _configFileProvider, cancellationToken);
        }
    }
}
