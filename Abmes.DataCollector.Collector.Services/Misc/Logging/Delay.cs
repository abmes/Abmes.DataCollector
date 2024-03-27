using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Misc.Logging;

public class Delay(
    ILogger<Delay> logger,
    IDelay delay) : IDelay
{
    public async Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken)
    {
        if (timeSpan.TotalSeconds > 0)
        {
            logger.LogInformation($"Waiting {reason} for {timeSpan.TotalSeconds} seconds ...");
        }

        await delay.DelayAsync(timeSpan, reason, cancellationToken);
    }
}
