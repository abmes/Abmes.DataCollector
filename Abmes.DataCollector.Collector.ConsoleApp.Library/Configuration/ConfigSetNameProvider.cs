using Abmes.DataCollector.Collector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration;

public class ConfigSetNameProvider : IConfigSetNameProvider
{
    private readonly IBootstrapper _bootstrapper;

    public ConfigSetNameProvider(IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }

    public string GetConfigSetName()
    {
        var args = Environment.GetCommandLineArgs();

        return string.IsNullOrEmpty(_bootstrapper.ConfigSetName) ? ((args.Length <= 1) ? string.Empty : args[1].Split('/', '\\').Last()) : _bootstrapper.ConfigSetName;
    }
}
