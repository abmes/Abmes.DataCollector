using Abmes.DataCollector.Collector.Services.Contracts.AppConfig;
using Abmes.DataCollector.Collector.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.App.Library.Configuration;

public class ConfigSetNameProvider(
    IBootstrapper bootstrapper) : IConfigSetNameProvider
{
    public string GetConfigSetName()
    {
        var args = Environment.GetCommandLineArgs();

        return
            !string.IsNullOrEmpty(bootstrapper.ConfigSetName)
            ? bootstrapper.ConfigSetName
            : args.Length <= 1
                ? string.Empty
                : args[1].Split('/', '\\').Last();
    }
}
