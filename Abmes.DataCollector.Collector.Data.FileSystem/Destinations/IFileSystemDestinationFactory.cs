using Abmes.DataCollector.Collector.Services.Configuration;

namespace Abmes.DataCollector.Collector.Data.FileSystem.Destinations;

public delegate IFileSystemDestination IFileSystemDestinationFactory(DestinationConfig destinationConfig);
