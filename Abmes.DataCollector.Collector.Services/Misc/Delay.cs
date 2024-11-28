using Abmes.DataCollector.Collector.Services.Ports.Misc;

namespace Abmes.DataCollector.Collector.Services.Misc;

public class Delay : IDelay
{
    public async Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken)
    {
        await Task.Delay(timeSpan, cancellationToken);
    }
}
