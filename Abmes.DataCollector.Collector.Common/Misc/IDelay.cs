namespace Abmes.DataCollector.Collector.Common.Misc
{
    public interface IDelay
    {
        Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken);
    }
}
