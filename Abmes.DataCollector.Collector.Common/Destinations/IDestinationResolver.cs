using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestinationResolver
    {
        IDestination GetDestination(DestinationConfig destinationConfig);
        bool CanResolve(DestinationConfig destinationConfig);
    }
}
