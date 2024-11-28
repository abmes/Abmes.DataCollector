using Abmes.DataCollector.Common.Data.Amazon.Configuration;
using Abmes.DataCollector.Common.Data.Amazon.Storage;
using Abmes.DataCollector.Common.Services.Ports.Configuration;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Common.Data.Amazon;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
    {
        if (AmazonRegistrationNeeded(configuration))
        {
            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
            builder.RegisterType<AmazonCommonStorage>().As<IAmazonCommonStorage>();
        }
    }

    public static bool AmazonRegistrationNeeded(IConfiguration configuration)
    {
        return
            (string.Equals(configuration.GetSection("AppSettings")?.GetValue<string>("ConfigStorageType"), "Amazon") ||
            (configuration.GetAWSOptions().Region is not null));
    }
}
