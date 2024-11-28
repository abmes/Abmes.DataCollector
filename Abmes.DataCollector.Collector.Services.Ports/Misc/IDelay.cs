namespace Abmes.DataCollector.Collector.Services.Ports.Misc;

public interface IDelay
{
    Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken);
}
