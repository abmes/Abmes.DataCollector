using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Utils.DependancyInjection
{
    static class OptionsAdapter
    {
        public static T Adapt<T>(IOptions<T> o) where T : class, new()
        {
            return o.Value;
        }
    }
}
