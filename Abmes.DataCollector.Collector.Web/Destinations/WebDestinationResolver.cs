using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Web.Destinations
{
    public class WebDestinationResolver : IDestinationResolver
    {
        private readonly IWebDestinationFactory _webDestinationFactory;

        public WebDestinationResolver(
            IWebDestinationFactory webDestinationFactory)
        {
            _webDestinationFactory = webDestinationFactory;
        }

        public bool CanResolve(DestinationConfig destinationConfig)
        {
            return string.Equals(destinationConfig.DestinationType, "Web");
        }

        public IDestination GetDestination(DestinationConfig destinationConfig)
        {
            return _webDestinationFactory(destinationConfig);
        }
    }
}
