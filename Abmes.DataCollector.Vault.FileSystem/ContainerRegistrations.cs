using Abmes.DataCollector.Vault.FileSystem.Storage;
using Abmes.DataCollector.Vault.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.FileSystem
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<FileSystemStorage>().As<IFileSystemStorage>();
            builder.RegisterType<FileSystemStorageResolver>().Named<IStorageResolver>("base");
        }
    }
}
