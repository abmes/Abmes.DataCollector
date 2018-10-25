using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestinationFactory
    {
        IDestination GetDestination(DestinationConfig destinationConfig);
    }
}
