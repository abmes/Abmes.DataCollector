﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Configuration
{
    public interface IConfigProvider
    {
        Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
    }
}
