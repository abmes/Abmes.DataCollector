using Abmes.DataCollector.Collector.Data.Configuration;

namespace Abmes.DataCollector.Collector.Data.FileSystem.Destinations;

public delegate IFileSystemDestination IFileSystemDestinationFactory(DestinationConfig destinationConfig);
