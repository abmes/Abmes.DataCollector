﻿using Abmes.DataCollector.Common.Data.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Abmes.DataCollector.Common.Data;

public static class ServicesConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CommonAppSettings>(configuration.GetSection("AppSettings"));
        services.AddOptionsAdapter<ICommonAppSettings, CommonAppSettings>();
    }
}
