using Abmes.DataCollector.Common.Data.FileSystem.Configuration;
using Abmes.DataCollector.Common.Data.FileSystem.Storage;
using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Autofac;

namespace Abmes.DataCollector.Common.Data.FileSystem;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        builder.RegisterType<FileSystemCommonStorage>().As<IFileSystemCommonStorage>();
    }
}
