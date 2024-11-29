using Abmes.DataCollector.Shared.Data.Amazon;
using Abmes.DataCollector.Shared.Data.Azure;
using Abmes.DataCollector.Shared.Data.Configuration;
using Abmes.DataCollector.Shared.Data.FileSystem;
using Abmes.DataCollector.Shared.Services.DI;
using Abmes.DataCollector.Vault.Data;
using Abmes.DataCollector.Vault.Data.Amazon;
using Abmes.DataCollector.Vault.Data.AspNetCore;
using Abmes.DataCollector.Vault.Data.Azure;
using Abmes.DataCollector.Vault.Data.Empty;
using Abmes.DataCollector.Vault.Data.FileSystem;
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

        SharedConfigurationDataStartup.ConfigureServices(services, configuration);
        SharedAmazonDataStartup.ConfigureServices(services, configuration);
        SharedAzureDataStartup.ConfigureServices(services, configuration);
        SharedFileSystemDataStartup.ConfigureServices(services, configuration);

        SharedServicesStartup.ConfigureServices(services);

        VaultCommonDataStartup.ConfigureServices(services, configuration);
        VaultAspNetCoreDataStartup.ConfigureServices(services, configuration);
        VaultAmazonDataStartup.ConfigureServices(services, configuration);
        VaultAzureDataStartup.ConfigureServices(services, configuration);
        VaultFileSystemDataStartup.ConfigureServices(services, configuration);
        VaultEmptyDataStartup.ConfigureServices(services, configuration);

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
        SharedConfigurationDataStartup.ConfigureContainer(builder);
        SharedAmazonDataStartup.ConfigureContainer(builder, configuration);
        SharedAzureDataStartup.ConfigureContainer(builder);
        SharedFileSystemDataStartup.ConfigureContainer(builder);

        SharedServicesStartup.ConfigureContainer(builder);

        VaultCommonDataStartup.ConfigureContainer(builder);
        VaultAspNetCoreDataStartup.ConfigureContainer(builder);
        VaultAmazonDataStartup.ConfigureContainer(builder);
        VaultAzureDataStartup.ConfigureContainer(builder);
        VaultFileSystemDataStartup.ConfigureContainer(builder);
        VaultEmptyDataStartup.ConfigureContainer(builder);

        VaultServicesStartup.ConfigureContainer(builder);

        builder.RegisterInstance(configuration).As<IConfiguration>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IEndpointRouteBuilder erb, IWebHostEnvironment env)
    {
        VaultAspNetCoreAppStartup.Configure(app, erb, env);
    }
}
