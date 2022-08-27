using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Web.Destinations
{
    public delegate IWebDestination IWebDestinationFactory(DestinationConfig destinationConfig);
}
