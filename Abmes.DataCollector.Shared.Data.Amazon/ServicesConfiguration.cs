using Abmes.DataCollector.Shared.Data.Amazon.Configuration;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Amazon;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();

        if (Abmes.DataCollector.Shared.Data.Amazon.ContainerRegistrations.AmazonRegistrationNeeded(configuration))
        {
            services.Configure<AmazonAppSettings>(configuration.GetSection("AppSettings"));
            services.AddOptionsAdapter<IAmazonAppSettings, AmazonAppSettings>();
        }
    }
}
