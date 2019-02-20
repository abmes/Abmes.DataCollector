using Abmes.DataCollector.Collector.Common.Destinations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Logging.Destinations
{
    public delegate ILoggingDestination ILoggingDestinationFactory(IDestination destination);
}
