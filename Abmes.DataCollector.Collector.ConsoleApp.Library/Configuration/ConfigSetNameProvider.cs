using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration;

public class ConfigSetNameProvider(
    IBootstrapper bootstrapper) : IConfigSetNameProvider
{
    public string GetConfigSetName()
    {
        var args = Environment.GetCommandLineArgs();

        return string.IsNullOrEmpty(bootstrapper.ConfigSetName) ? ((args.Length <= 1) ? string.Empty : args[1].Split('/', '\\').Last()) : bootstrapper.ConfigSetName;
    }
}
