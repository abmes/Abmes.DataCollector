using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.FileSystem.Destinations
{
    public delegate IFileSystemDestination IFileSystemDestinationFactory(DestinationConfig destinationConfig);
}
