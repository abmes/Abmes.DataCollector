using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Azure.Destinations
{
    public delegate IAzureDestination IAzureDestinationFactory(DestinationConfig destinationConfig);
}
