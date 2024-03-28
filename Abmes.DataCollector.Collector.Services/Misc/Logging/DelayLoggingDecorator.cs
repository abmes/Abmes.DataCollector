using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.Services.Misc.Logging;

public class DelayLoggingDecorator(
    ILogger<DelayLoggingDecorator> logger,
    IDelay delay) : IDelay
{
    public async Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken)
    {
        if (timeSpan.TotalSeconds > 0)
        {
            logger.LogInformation("Waiting {reason} for {seconds} seconds ...", reason, timeSpan.TotalSeconds);
        }

        await delay.DelayAsync(timeSpan, reason, cancellationToken);
    }
}
