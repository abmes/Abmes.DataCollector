using Abmes.DataCollector.Collector.Services.Abstractions.AppConfig;
using Abmes.DataCollector.Common.Data.Configuration;

namespace Abmes.DataCollector.Collector.App.Library.Configuration;

public class ConfigLocationProvider(
    IBootstrapper bootstrapper) : IConfigLocationProvider
{
    public string? GetConfigLocation()
    {
        var args = Environment.GetCommandLineArgs();

        if ((!string.IsNullOrEmpty(bootstrapper.ConfigSetName)) || (args.Length <= 1))
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
