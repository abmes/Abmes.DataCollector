﻿using Abmes.DataCollector.Collector.Services.Ports.AppConfig;
using Abmes.DataCollector.Shared.Services.Ports.Configuration;

namespace Abmes.DataCollector.Collector.Data.CommandLine.Configuration;

public class ConfigLocationProvider(
    IBootstrapper bootstrapper)
    : IConfigLocationProvider
{
    public string? GetConfigLocation()
    {
        var args = Environment.GetCommandLineArgs();

        if (!string.IsNullOrEmpty(bootstrapper.ConfigSetName) || args.Length <= 1)
        {
            return null;
        }


        var location = args[1];
        var bracketPos = location.IndexOf('[');

        var suffix = bracketPos >= 0 ? location[bracketPos..] : null;
        if (!string.IsNullOrEmpty(suffix))
        {
            location = location[..^suffix.Length];
        }

        return string.Join(Path.DirectorySeparatorChar, location.Split('/', '\\').SkipLast(1)) + suffix;
    }
}
