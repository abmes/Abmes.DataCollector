using Abmes.DataCollector.Collector.Data.FileSystem.Destinations;
using Abmes.DataCollector.Collector.Services.Ports.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.FileSystem;

public static class CollectorFileSystemDataStartup
{
    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystemDestination>().As<IFileSystemDestination>();
        builder.RegisterType<FileSystemDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
