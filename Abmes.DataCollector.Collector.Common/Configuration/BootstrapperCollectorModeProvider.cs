using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class BootstrapperCollectorModeProvider : ICollectorModeProvider
    {
        private readonly IBootstrapper _bootstrapper;

        public BootstrapperCollectorModeProvider(IBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public CollectorMode GetCollectorMode()
        {
            return _bootstrapper.CollectorMode;
        }
    }
}
