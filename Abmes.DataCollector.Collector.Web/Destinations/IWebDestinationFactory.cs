using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Web.Destinations
{
    public delegate IWebDestination IWebDestinationFactory(DestinationConfig destinationConfig);
}
