using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class BootstrapperTimeFilterProvider : ITimeFilterProvider
    {
        private readonly IBootstrapper _bootstrapper;

        public BootstrapperTimeFilterProvider(IBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
        }

        public string GetTimeFilter()
        {
            return _bootstrapper.TimeFilter;
        }
    }
}
