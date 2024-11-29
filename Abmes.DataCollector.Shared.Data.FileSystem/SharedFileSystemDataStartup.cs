using Abmes.DataCollector.Shared.Data.FileSystem.Configuration;
using Abmes.DataCollector.Shared.Data.FileSystem.Storage;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.FileSystem;

public static class SharedFileSystemDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileSystemAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IFileSystemAppSettings, FileSystemAppSettings>();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
        builder.RegisterType<FileSystemCommonStorage>().As<IFileSystemCommonStorage>();
    }
}
