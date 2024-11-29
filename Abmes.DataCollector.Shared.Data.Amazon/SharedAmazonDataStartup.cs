using Abmes.DataCollector.Shared.Data.Amazon.Configuration;
using Abmes.DataCollector.Shared.Data.Amazon.Storage;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Amazon.S3;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Amazon;

public static class SharedAmazonDataStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        if (IsAmazonRegistrationNeeded(configuration))
        {
            services.Configure<AmazonAppSettings>(configuration.GetSection("AppSettings"));
            services.AddOptionsAdapter<IAmazonAppSettings, AmazonAppSettings>();
        }
    }

    public static void ConfigureContainer(ContainerBuilder builder, IConfiguration configuration)
    {
        if (IsAmazonRegistrationNeeded(configuration))
        {
            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
            builder.RegisterType<AmazonCommonStorage>().As<IAmazonCommonStorage>();
        }
    }

    public static bool IsAmazonRegistrationNeeded(IConfiguration configuration)
    {
        return
            (string.Equals(configuration.GetSection("AppSettings")?.GetValue<string>("ConfigStorageType"), "Amazon") ||
            (configuration.GetAWSOptions().Region is not null));
    }
}
