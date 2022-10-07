using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Common.Configuration;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration;

public class ConfigLocationProvider : IConfigLocationProvider
{
    private readonly IBootstrapper _bootstrapper;

    public ConfigLocationProvider(IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;
    }

    public string? GetConfigLocation()
    {
        var args = Environment.GetCommandLineArgs();

        if ((_bootstrapper.ConfigSetName is not null) || (args.Length <= 1))
        {
            return null;
        }


        var location = args[1];
        var bracketPos = location.IndexOf('[');

        var suffix = (bracketPos >= 0) ? location[bracketPos..] : null;
        if (!string.IsNullOrEmpty(suffix))
        {
            location = location[..^suffix.Length];
        }

        return string.Join(System.IO.Path.DirectorySeparatorChar, location.Split('/', '\\').SkipLast(1)) + suffix;
    }
}
