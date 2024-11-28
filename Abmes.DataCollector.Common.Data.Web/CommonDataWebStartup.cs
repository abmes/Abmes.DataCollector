using Abmes.DataCollector.Common.Data.Web.Configuration;
using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Common.Data.Web;

public static class CommonDataWebStartup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
    }
}
