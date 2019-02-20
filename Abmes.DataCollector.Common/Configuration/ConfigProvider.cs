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
        private readonly ICommonAppSettings _commonAppSettings;

        public ConfigProvider(
            IEnumerable<IConfigLoader> configLoaders,
            ICommonAppSettings commonAppSettings)
        {
            _configLoaders = configLoaders;
            _commonAppSettings = commonAppSettings;
        }

        public async Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_commonAppSettings.ConfigStorageType))
            {
                throw new Exception("ConfigStorageType not specified!");
            }

            var configLoader = _configLoaders.Where(x => x.CanLoadFrom(_commonAppSettings.ConfigStorageType)).FirstOrDefault();

            if (configLoader == null)
            {
                throw new Exception($"Can't provide configuration from storage type '{_commonAppSettings.ConfigStorageType}'");
            }

            return await configLoader.GetConfigContentAsync(configName, cancellationToken);
        }
    }
}
