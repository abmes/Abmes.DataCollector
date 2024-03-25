namespace Abmes.DataCollector.Collector.Services.Misc;

public interface IDelay
{
    Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken);
}
