using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization;

public static class Initializer
{
    public static IMainService GetMainService()
    {
        var startup = new Startup();
        var services = new ServiceCollection();

        startup.ConfigureServices(services);

        var builder = new ContainerBuilder();
        builder.Populate(services);

        startup.ConfigureContainer(builder);

        var container = builder.Build();
        var serviceProvider = container.Resolve<IServiceProvider>();

        return serviceProvider.GetRequiredService<IMainService>();
    }
}
