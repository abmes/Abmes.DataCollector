﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface IMainCollector
    {
        Task<bool> CollectAsync(string configSetName, CancellationToken cancellationToken);
    }
}
