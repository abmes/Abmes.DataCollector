using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Service
{
    public static class WebHostBuilderInit
    {
        public static IWebHostBuilder InitWebHostBuilder(this IWebHostBuilder builder)
        {
            return
                builder
                    .UseStartup<Startup>();
        }
    }
}
