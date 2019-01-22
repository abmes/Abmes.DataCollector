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

        private string ResultPrefix(bool result)
        {
            return (result ? "Finished" : "ERRORS occured when");
        }

        public async Task<bool> CollectAsync(CancellationToken cancellationToken)
        {

            try
            {
                bool result;

                _logger.LogInformation("Started collecting data collections.");

                var watch = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    result = await _collector.CollectAsync(cancellationToken);
                }
                finally
                {
                    watch.Stop();
                }

                _logger.LogInformation(ResultPrefix(result) + " collecting data collections. Elapsed time: {elapsed}", watch.Elapsed);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogCritical(ResultPrefix(false) + " collecting data collections: {errorMessage}", e.GetAggregateMessages());
                throw;
            }
        }
    }
}
