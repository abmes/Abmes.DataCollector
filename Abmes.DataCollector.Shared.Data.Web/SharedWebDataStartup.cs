using Abmes.DataCollector.Shared.Data.Web.Configuration;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Shared.Data.Web;

public static class SharedWebDataStartup
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
