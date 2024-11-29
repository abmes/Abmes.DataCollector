using Abmes.DataCollector.Vault.Data.AspNetCore;
using Abmes.DataCollector.Vault.Services.DI;
using Abmes.DataCollector.Vault.Web.AspNetCore;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Vault.Web.Library;

public class Startup(
    IConfiguration configuration)
{
    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(loggingBuilder => loggingBuilder.AddSimpleConsole());

        services.AddSingleton<TimeProvider>(TimeProvider.System);

        VaultAspNetCoreDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Configuration.SharedConfigurationDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.Azure.SharedAzureDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Shared.Data.FileSystem.SharedFileSystemDataStartup.ConfigureServices(services, configuration);
        Abmes.DataCollector.Vault.Data.Empty.VaultEmptyDataStartup.ConfigureServices(services, configuration);
        Data.VaultCommonDataStartup.ConfigureServices(services, configuration);
        VaultAspNetCoreAppStartup.ConfigureServices(services, configuration);
        VaultServicesStartup.ConfigureServices(services, configuration);
    }

    // ConfigureContainer is where you can register things directly
    // with Autofac. This runs after ConfigureServices so the things
    // here will override registrations made in ConfigureServices.
    // Don't build the container; that gets done for you. If you
    // need a reference to the container, you need to use the
    // "Without ConfigureContainer" mechanism shown later.
    public void ConfigureContainer(ContainerBuilder builder)
    {
        Abmes.DataCollector.Shared.Data.Configuration.SharedConfigurationDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Data.Amazon.SharedAmazonDataStartup.ConfigureContainer(builder, configuration);
        Abmes.DataCollector.Shared.Data.Azure.SharedAzureDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Data.FileSystem.SharedFileSystemDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Shared.Services.DI.SharedServicesStartup.ConfigureContainer(builder);

        VaultAspNetCoreDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Vault.Data.Amazon.VaultAmazonDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Vault.Data.Azure.ValultAzureDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Vault.Data.FileSystem.VaultFileSystemDataStartup.ConfigureContainer(builder);
        Abmes.DataCollector.Vault.Data.Empty.VaultEmptyDataStartup.ConfigureContainer(builder);
        VaultServicesStartup.ConfigureContainer(builder);

        builder.RegisterInstance(configuration).As<IConfiguration>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder erb, IWebHostEnvironment env)
    {
        VaultAspNetCoreAppStartup.Configure(app, erb, env);
    }
}
