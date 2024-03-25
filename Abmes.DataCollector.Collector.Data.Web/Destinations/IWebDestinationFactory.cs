using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Data.Web.Destinations;

public delegate IWebDestination IWebDestinationFactory(DestinationConfig destinationConfig);
