using Abmes.DataCollector.Collector.Services.Configuration;
using Abmes.DataCollector.Collector.Services.Destinations;

namespace Abmes.DataCollector.Collector.Data.FileSystem.Destinations;

public class FileSystemDestinationResolver(
    IFileSystemDestinationFactory FileSystemDestinationFactory) : IDestinationResolver
{
    public bool CanResolve(DestinationConfig destinationConfig)
    {
        return string.Equals(destinationConfig.DestinationType, "FileSystem");
    }

    public IDestination GetDestination(DestinationConfig destinationConfig)
    {
        return FileSystemDestinationFactory(destinationConfig);
    }
}
