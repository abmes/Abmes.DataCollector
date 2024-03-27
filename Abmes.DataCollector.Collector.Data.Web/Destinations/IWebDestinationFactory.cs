using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Web.Destinations;

public delegate IWebDestination IWebDestinationFactory(DestinationConfig destinationConfig);
