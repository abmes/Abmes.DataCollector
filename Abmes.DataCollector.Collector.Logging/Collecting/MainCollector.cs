using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Common.Collecting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Collector.Logging.Collecting
{
    public class MainCollector : IMainCollector
    {
        private readonly ILogger<MainCollector> _logger;
        private readonly IMainCollector _collector;

        public MainCollector(ILogger<MainCollector> logger, IMainCollector collector)
        {
            _logger = logger;
            _collector = collector;
        }

        public async Task CollectAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Started collecting data collections.");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    await _collector.CollectAsync(cancellationToken);
                }
                finally
                {
                    watch.Stop();
                }

                _logger.LogInformation("Finished collecting data collections. Elapsed time: {elapsed}", watch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error collecting data collections: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }
    }
}
