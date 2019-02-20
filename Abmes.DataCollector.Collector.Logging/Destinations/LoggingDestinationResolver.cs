using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Logging.Destinations
{
    public class LoggingDestinationResolver : IDestinationResolver
    {
        private readonly IDestinationResolver _destinationResolver;
        private readonly ILoggingDestinationFactory _loggingDestinationFactory;

        public LoggingDestinationResolver(
            IDestinationResolver DestinationResolver,
            ILoggingDestinationFactory loggingDestinationFactory)
        {
            _destinationResolver = DestinationResolver;
            _loggingDestinationFactory = loggingDestinationFactory;
        }

        public bool CanResolve(DestinationConfig destinationConfig)
        {
            return _destinationResolver.CanResolve(destinationConfig);
        }

        public IDestination GetDestination(DestinationConfig destinationConfig)
        {
            var destination = _destinationResolver.GetDestination(destinationConfig);
            return _loggingDestinationFactory(destination);
        }
    }
}
