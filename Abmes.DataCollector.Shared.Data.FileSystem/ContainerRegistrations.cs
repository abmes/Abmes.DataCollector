using Abmes.DataCollector.Shared.Data.FileSystem.Configuration;
using Abmes.DataCollector.Shared.Data.FileSystem.Storage;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;

namespace Abmes.DataCollector.Shared.Data.FileSystem;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        builder.RegisterType<FileSystemCommonStorage>().As<IFileSystemCommonStorage>();
    }
}
