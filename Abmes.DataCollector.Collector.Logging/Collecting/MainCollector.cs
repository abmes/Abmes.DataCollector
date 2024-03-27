using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Services.Collecting;
using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Utils;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Logging.Collecting;

public class MainCollector(
    ILogger<MainCollector> logger,
    IMainCollector collector,
    IConfigSetNameProvider configSetNameProvider,
    ICollectorModeProvider collectorModeProvider) : IMainCollector
{
    private static string ResultPrefix(bool result)
    {
        return (result ? "Finished" : "ERRORS occured when");
    }

    public async Task<IEnumerable<string>> CollectAsync(CancellationToken cancellationToken)
    {
        var configSetName = configSetNameProvider.GetConfigSetName();
        var collectorMode = collectorModeProvider.GetCollectorMode();

        var mode = collectorMode.ToString().ToLowerInvariant();

        try
        {
            IEnumerable<string> result;

            logger.LogInformation($"[{configSetName}] Started {mode}ing data collections.");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                result = await collector.CollectAsync(cancellationToken);
            }
            finally
            {
                watch.Stop();
            }

            var failedDataCollectionNames = result.Any() ? " (" + string.Join(",", result) + ")" : null;

            logger.LogInformation($"[{configSetName}] {ResultPrefix(!result.Any())} {mode}ing data collections{failedDataCollectionNames}. Elapsed time: {watch.Elapsed}");

            return result;
        }
        catch (Exception e)
        {
            logger.LogCritical($"[{configSetName}] " + ResultPrefix(false) + " {mode}ing data collections: {errorMessage}", mode, e.GetAggregateMessages());
            throw;
        }
    }
}
