using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
