using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.Web.Destinations;

public delegate IWebDestination IWebDestinationFactory(DestinationConfig destinationConfig);
