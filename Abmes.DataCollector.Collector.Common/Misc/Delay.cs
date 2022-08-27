namespace Abmes.DataCollector.Collector.Common.Misc
{
    public class Delay : IDelay
    {
        public async Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken)
        {
            await Task.Delay(timeSpan, cancellationToken);
        }
    }
}
