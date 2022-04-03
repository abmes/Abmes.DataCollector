using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Misc
{
    public interface ITimeFilterProcessor
    {
        bool TimeFilterAccepted(string timeFilter);
    }
}
