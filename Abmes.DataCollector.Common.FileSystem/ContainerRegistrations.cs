using Abmes.DataCollector.Common.FileSystem.Configuration;
using Abmes.DataCollector.Common.FileSystem.Storage;
using Abmes.DataCollector.Common.Configuration;
using Autofac;

namespace Abmes.DataCollector.Common.FileSystem
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
            builder.RegisterType<FileSystemCommonStorage>().As<IFileSystemCommonStorage>();
        }
    }
}
