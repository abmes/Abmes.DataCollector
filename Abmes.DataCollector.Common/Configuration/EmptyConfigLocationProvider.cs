using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Configuration
{
    public class EmptyConfigLocationProvider : IConfigLocationProvider
    {
        public string GetConfigLocation()
        {
            return null;
        }
    }
}
