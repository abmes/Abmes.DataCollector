﻿using Abmes.DataCollector.Collector.Data.Destinations;
using Abmes.DataCollector.Collector.Data.FileSystem.Destinations;
using Autofac;

namespace Abmes.DataCollector.Collector.Data.FileSystem;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystemDestination>().As<IFileSystemDestination>();
        builder.RegisterType<FileSystemDestinationResolver>().Named<IDestinationResolver>("base");
    }
}
