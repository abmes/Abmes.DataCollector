﻿using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.Console.Destinations;

public delegate IConsoleDestination IConsoleDestinationFactory(DestinationConfig destinationConfig);
