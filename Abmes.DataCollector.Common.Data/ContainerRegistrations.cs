using Abmes.DataCollector.Common.Data.Configuration;
using Abmes.DataCollector.Common.Data.Storage;
using Autofac;

namespace Abmes.DataCollector.Common.Data;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");
        builder.RegisterType<FileNameProvider>().As<IFileNameProvider>();
    }
}
