using Abmes.DataCollector.Collector.Logging;
using Abmes.DataCollector.Utils.AspNetCore;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Abmes.DataCollector.Collector.Service;

public class Startup(
    IConfiguration configuration)
{
    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddHttpContextAccessor();

        services.AddHealthChecks();

        services.AddOptions();
        services.AddHttpClient();
        services.AddLogging(loggingBuilder =>
        {
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


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // order of middlewares is important
        app.UseMiddleware<BasicExceptionHandlingMiddleware>();
        app.UseMiddleware<ExceptionLoggingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/Health");
        });
    }
}
