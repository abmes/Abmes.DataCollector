﻿using Abmes.DataCollector.Collector.Common.Configuration;
using Abmes.DataCollector.Collector.Services.Abstractions.AppConfig;

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
