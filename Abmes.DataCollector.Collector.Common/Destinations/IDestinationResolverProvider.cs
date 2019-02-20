using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestinationResolverProvider
    {
        IDestinationResolver GetResolver(DestinationConfig destinationConfig);
    }
}
