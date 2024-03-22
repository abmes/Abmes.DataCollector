using Abmes.DataCollector.Vault.Data.FileSystem.Storage;
using Abmes.DataCollector.Vault.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Data.FileSystem;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystemStorage>().As<IFileSystemStorage>();
        builder.RegisterType<FileSystemStorageResolver>().Named<IStorageResolver>("base");
    }
}
