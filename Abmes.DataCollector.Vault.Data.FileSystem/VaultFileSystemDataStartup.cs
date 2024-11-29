using Abmes.DataCollector.Vault.Data.FileSystem.Storage;
using Abmes.DataCollector.Vault.Services.Ports.Storage;
using Autofac;

namespace Abmes.DataCollector.Vault.Data.FileSystem;

public static class VaultFileSystemDataStartup
{
    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<FileSystemStorage>().As<IFileSystemStorage>();
        builder.RegisterType<FileSystemStorageResolver>().Named<IStorageResolver>("base");
    }
}
