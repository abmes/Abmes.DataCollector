using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Common.Storage;
using Autofac;

namespace Abmes.DataCollector.Common
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigProvider>().Named<IConfigProvider>("base");
            builder.RegisterType<FileNameProvider>().As<IFileNameProvider>();
            builder.RegisterType<FileInfoData>().As<IFileInfoData>();
        }
    }
}
