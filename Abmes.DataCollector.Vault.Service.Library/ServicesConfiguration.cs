using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Service
{
    public static class ServicesConfiguration
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            Abmes.DataCollector.Common.Amazon.ServicesConfiguration.Configure(services, configuration);
            Abmes.DataCollector.Vault.ServicesConfiguration.Configure(services, configuration);
        }
    }
}
