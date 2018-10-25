using Abmes.DataCollector.Common.Storage;
using Autofac;

namespace Abmes.DataCollector.Common
{
    public static class ContainerRegistrations
    {
        public static void RegisterFor(ContainerBuilder builder)
        {
            builder.RegisterType<FileNameProvider>().As<IFileNameProvider>();
        }
    }
}
