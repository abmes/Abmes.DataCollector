
using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Utils.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Common
{
    public static class ServicesConfiguration
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CommonAppSettings>(configuration.GetSection("AppSettings"));
            services.AddOptionsAdapter<ICommonAppSettings, CommonAppSettings>();
        }
    }
}
