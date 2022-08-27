using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class MainCollector : IMainCollector
    {
        private readonly ILogger<MainCollector> _logger;
        private readonly IMainCollector _collector;
        private readonly ICollectorModeProvider _collectorModeProvider;

        public IConfigSetNameProvider _configSetNameProvider { get; }

        public MainCollector(
            ILogger<MainCollector> logger, 
            IMainCollector collector,
            IConfigSetNameProvider configSetNameProvider,
            ICollectorModeProvider collectorModeProvider)
        {
            _logger = logger;
            _collector = collector;
            _configSetNameProvider = configSetNameProvider;
            _collectorModeProvider = collectorModeProvider;
        }

        private string ResultPrefix(bool result)
        {
            return (result ? "Finished" : "ERRORS occured when");
        }

        public async Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken)
        {
            var configSetName = _configSetNameProvider.GetConfigSetName();
            var collectorMode = _collectorModeProvider.GetCollectorMode();

            var mode = collectorMode.ToString().ToLowerInvariant();

            try
            {
                IEnumerable<string> result;

                _logger.LogInformation($"[{configSetName}] Started {mode}ing data collections.");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    result = await _collector.CollectAsync(cancellationToken);
                }
                finally
                {
                    watch.Stop();
                }

                var failedDataCollectionNames = result.Any() ? " (" + string.Join(",", result) + ")" : null;

                _logger.LogInformation($"[{configSetName}] {ResultPrefix(!result.Any())} {mode}ing data collections{failedDataCollectionNames}. Elapsed time: {watch.Elapsed}");

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"[{configSetName}] " + ResultPrefix(false) + " {mode}ing data collections: {errorMessage}", mode, e.GetAggregateMessages());
                throw;
            }
        }
    }
}
