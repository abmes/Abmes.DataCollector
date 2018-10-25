using Abmes.DataCollector.Common.Amazon.Configuration;
using Abmes.DataCollector.Utils.DependancyInjection;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Common.Amazon
{
    public static class ServicesConfiguration
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            services.Configure<AmazonAppSettings>(configuration.GetSection("AppSettings"));
            services.AddOptionsAdapter<IAmazonAppSettings, AmazonAppSettings>();
        }
    }
}
