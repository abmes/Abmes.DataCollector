using Abmes.DataCollector.Common.Amazon.Configuration;
using Abmes.DataCollector.Common.Amazon.Storage;
using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Abmes.DataCollector.Common.Amazon
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder, IConfiguration configuration)
        {
            var awsOptions = configuration.GetAWSOptions();

            if (awsOptions.Region == null)
            {
                return;
            }

            builder.RegisterType<ConfigLoader>().As<IConfigLoader>();
            builder.RegisterType<AmazonCommonStorage>().As<IAmazonCommonStorage>();
        }
    }
}
