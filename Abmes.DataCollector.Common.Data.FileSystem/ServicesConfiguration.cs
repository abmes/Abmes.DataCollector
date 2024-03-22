using Abmes.DataCollector.Common.Data.FileSystem.Configuration;
using Abmes.DataCollector.Utils.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Common.Data.FileSystem;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FileSystemAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<IFileSystemAppSettings, FileSystemAppSettings>();
    }
}
