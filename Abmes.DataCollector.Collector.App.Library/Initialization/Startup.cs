using Abmes.DataCollector.Collector.Logging.Amazon;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.App.Library.Initialization;

public class Startup
{
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        configuration = builder.Build();
    }

    private readonly IConfiguration configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(configuration);

        services.AddOptions();
        services.AddHttpClient();
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSimpleConsole();
            loggingBuilder.AddDebug();

            LoggingConfigurator.Configure(loggingBuilder, configuration);
        });

        ServicesConfiguration.Configure(services, configuration);
    }

    // ConfigureContainer is where you can register things directly
    // with Autofac. This runs after ConfigureServices so the things
    // here will override registrations made in ConfigureServices.
    // Don't build the container; that gets done for you. If you
    // need a reference to the container, you need to use the
    // "Without ConfigureContainer" mechanism shown later.
    public void ConfigureContainer(ContainerBuilder builder)
    {
        // Register your own things directly with Autofac
        ContainerRegistrations.RegisterFor(builder, configuration);
        builder.RegisterInstance(configuration).As<IConfiguration>();
        //...
    }
}
