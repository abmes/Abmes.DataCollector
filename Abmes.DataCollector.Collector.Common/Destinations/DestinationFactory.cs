using Autofac.Features.Indexed;
using Abmes.DataCollector.Collector.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public class DestinationFactory : IDestinationFactory
    {
        private readonly IIndex<string, IDestination> _factory;

        public DestinationFactory(IIndex<string, IDestination> factory)
        {
            _factory = factory;
        }

        public IDestination GetDestination(DestinationConfig destinationConfig)
        {
            var result = _factory[destinationConfig.DestinationType];
            result.DestinationConfig = destinationConfig;

            return result;
        }
    }
}
