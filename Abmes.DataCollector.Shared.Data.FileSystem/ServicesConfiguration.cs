using Abmes.DataCollector.Shared.Data.FileSystem.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.FileSystem;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileSystemAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IFileSystemAppSettings, FileSystemAppSettings>();
    }
}
