using Abmes.DataCollector.Collector.Services.Ports.Destinations;

namespace Abmes.DataCollector.Collector.Data.FileSystem.Destinations;

public delegate IFileSystemDestination IFileSystemDestinationFactory(DestinationConfig destinationConfig);
