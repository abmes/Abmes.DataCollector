using Abmes.DataCollector.Collector.FileSystem.Destinations;
using Abmes.DataCollector.Collector.Common.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.FileSystem;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystemDestination>().As<IFileSystemDestination>();
        builder.RegisterType<FileSystemDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
