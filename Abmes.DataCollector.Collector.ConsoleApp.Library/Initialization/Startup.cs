using Abmes.DataCollector.Collector.Logging;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

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

        private IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddOptions();
            services.AddLogging();

            ServicesConfiguration.Configure(services, Configuration);

            var builder = new ContainerBuilder();

            builder.Populate(services);
            ContainerRegistrations.RegisterFor(builder);

            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }

        public void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            LoggingConfigurator.Configure(loggerFactory);
        }
    }
}
