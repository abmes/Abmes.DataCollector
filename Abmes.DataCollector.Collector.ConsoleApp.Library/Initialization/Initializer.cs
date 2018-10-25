using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.ConsoleApp.Initialization
{
    public static class Initializer
    {
        public static IMainService GetMainService()
        {
            var startup = new Startup();
            var serviceProvider = startup.ConfigureServices(new ServiceCollection());
            startup.Configure(serviceProvider.GetService<ILoggerFactory>());

            return serviceProvider.GetService<IMainService>();
        }
    }
}
