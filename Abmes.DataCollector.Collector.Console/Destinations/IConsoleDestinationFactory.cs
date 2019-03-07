using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Console.Destinations
{
    public delegate IConsoleDestination IConsoleDestinationFactory(DestinationConfig destinationConfig);
}
