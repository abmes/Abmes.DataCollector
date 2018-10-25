using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Misc
{
    public interface IDelay
    {
        Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken);
    }
}
