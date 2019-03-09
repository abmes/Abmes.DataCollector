using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Common.Collecting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class MainCollector : IMainCollector
    {
        private readonly ILogger<MainCollector> _logger;
        private readonly IMainCollector _collector;

        public IConfigSetNameProvider _configSetNameProvider { get; }

        public MainCollector(
            ILogger<MainCollector> logger, 
            IMainCollector collector,
            IConfigSetNameProvider configSetNameProvider)
        {
            _logger = logger;
            _collector = collector;
            _configSetNameProvider = configSetNameProvider;
        }

        private string ResultPrefix(bool result)
        {
            return (result ? "Finished" : "ERRORS occured when");
        }

        public async Task<bool> CollectAsync(CancellationToken cancellationToken)
        {
            var configSetName = _configSetNameProvider.GetConfigSetName();

            try
            {
                bool result;

                _logger.LogInformation($"[{configSetName}] Started collecting data collections.");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    result = await _collector.CollectAsync(cancellationToken);
                }
                finally
                {
                    watch.Stop();
                }

                _logger.LogInformation($"[{configSetName}] " + ResultPrefix(result) + " collecting data collections. Elapsed time: {elapsed}", watch.Elapsed);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"[{configSetName}] " + ResultPrefix(false) + " collecting data collections: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }
    }
}
