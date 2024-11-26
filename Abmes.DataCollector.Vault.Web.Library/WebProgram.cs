using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Abmes.DataCollector.Vault.Web.Library;

public static class WebProgram
{
    public static void Run(string[] args, Action<IServiceCollection>? configureServices = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseServiceProviderFactory(new Autofac.Extensions.DependencyInjection.AutofacServiceProviderFactory());

        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);
        configureServices?.Invoke(builder.Services);

        builder.Host.ConfigureContainer<Autofac.ContainerBuilder>(startup.ConfigureContainer);

        var app = builder.Build();

        startup.Configure(app, app, app.Environment);

        app.Run();
    }
}
