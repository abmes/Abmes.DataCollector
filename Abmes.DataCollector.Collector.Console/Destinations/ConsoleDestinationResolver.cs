using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Console.Destinations
{
    public class ConsoleDestinationResolver : IDestinationResolver
    {
        private readonly IConsoleDestinationFactory _webDestinationFactory;

        public ConsoleDestinationResolver(
            IConsoleDestinationFactory webDestinationFactory)
        {
            _webDestinationFactory = webDestinationFactory;
        }

        public bool CanResolve(DestinationConfig destinationConfig)
        {
            return string.Equals(destinationConfig.DestinationType, "Console");
        }

        public IDestination GetDestination(DestinationConfig destinationConfig)
        {
            return _webDestinationFactory(destinationConfig);
        }
    }
}
