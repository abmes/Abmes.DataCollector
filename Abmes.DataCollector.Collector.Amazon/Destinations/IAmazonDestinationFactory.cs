using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Amazon.Destinations
{
    public delegate IAmazonDestination IAmazonDestinationFactory(DestinationConfig destinationConfig);
}
