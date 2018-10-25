using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestinationProvider
    {
        Task<IDestination> GetDestinationAsync(string destinationId, CancellationToken cancellationToken);
    }
}
