using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class BootstrapperConfigSetNameProvider : IConfigSetNameProvider
    {
        private readonly IBootstrapper _bootstrapper;

        public BootstrapperConfigSetNameProvider(IBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public string GetConfigSetName()
        {
            return _bootstrapper.ConfigSetName;
        }
    }
}
