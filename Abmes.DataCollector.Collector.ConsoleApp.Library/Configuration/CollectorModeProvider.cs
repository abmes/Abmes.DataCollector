using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.ConsoleApp.Configuration
{
    public class CollectorModeProvider : ICollectorModeProvider
    {
        private readonly IBootstrapper _bootstrapper;

        public CollectorModeProvider(IBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public CollectorMode GetCollectorMode()
        {
            var args = Environment.GetCommandLineArgs();

            return
                (_bootstrapper.CollectorMode != CollectorMode.None) ?
                _bootstrapper.CollectorMode :
                ((args.Length > 3) ? Enum.Parse<CollectorMode>(args[3], true) : CollectorMode.Collect);
        }
    }
}
