using Abmes.DataCollector.Collector.Common.Configuration;
using System;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration
{
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

            return (args.Length > 1) ? args[1] : _bootstrapper.ConfigSetName;
        }
    }
}
