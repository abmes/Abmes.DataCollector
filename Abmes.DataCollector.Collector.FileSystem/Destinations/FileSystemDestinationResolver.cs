using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Common.Destinations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.FileSystem.Destinations
{
    public class FileSystemDestinationResolver : IDestinationResolver
    {
        private readonly IFileSystemDestinationFactory _fileSystemDestinationFactory;

        public FileSystemDestinationResolver(
            IFileSystemDestinationFactory FileSystemDestinationFactory)
        {
            _fileSystemDestinationFactory = FileSystemDestinationFactory;
        }

        public bool CanResolve(DestinationConfig destinationConfig)
        {
            return string.Equals(destinationConfig.DestinationType, "FileSystem");
        }

        public IDestination GetDestination(DestinationConfig destinationConfig)
        {
            return _fileSystemDestinationFactory(destinationConfig);
        }
    }
}
