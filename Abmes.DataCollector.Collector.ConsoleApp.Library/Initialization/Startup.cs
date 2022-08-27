using Abmes.DataCollector.Collector.Logging;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddOptions();
            services.AddHttpClient();
            services.AddLogging(loggingBuilder =>
            {
                //var loggingConfigSection = Configuration.GetSection(Configuration["Logging"]);
                //loggingBuilder.AddConfiguration(loggingConfigSection);
                //loggingBuilder.AddConsole();
                loggingBuilder.AddProvider(new Logging.CollectorConsoleLoggingProvider());
                loggingBuilder.AddDebug();

                LoggingConfigurator.Configure(loggingBuilder, Configuration);
            });

            ServicesConfiguration.Configure(services, Configuration);
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
            ContainerRegistrations.RegisterFor(builder, Configuration);
            builder.RegisterInstance(Configuration).As<IConfiguration>();
            //...
        }
    }
}
