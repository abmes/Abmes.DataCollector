using Abmes.DataCollector.Common.Services.Storage;
using Autofac;

namespace Abmes.DataCollector.Common.Services.DI;

public static class ContainerRegistrations
{
    public static void RegisterFor(ContainerBuilder builder)
    {
        builder.RegisterType<FileNameProvider>().As<IFileNameProvider>();
    }
}
