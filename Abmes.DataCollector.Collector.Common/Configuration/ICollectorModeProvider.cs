﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public interface ICollectorModeProvider
    {
        CollectorMode GetCollectorMode();
    }
}
