using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.Data.FileSystem.Destinations;

public delegate IFileSystemDestination IFileSystemDestinationFactory(DestinationConfig destinationConfig);
