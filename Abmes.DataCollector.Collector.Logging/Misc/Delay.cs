using Microsoft.Extensions.Logging;
using Abmes.DataCollector.Collector.Common.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Logging.Misc
{
    public class Delay : IDelay
    {
        private readonly ILogger<Delay> _logger;
        private readonly IDelay _delay;

        public Delay(ILogger<Delay> logger, IDelay delay)
        {
            _logger = logger;
            _delay = delay;
        }

        public async Task DelayAsync(TimeSpan timeSpan, string reason, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Waiting {reason} for {timeSpan.TotalSeconds} seconds ...");
            await _delay.DelayAsync(timeSpan, reason, cancellationToken);
        }
    }
}
