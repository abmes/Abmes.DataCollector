﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Common.Configuration
{
    public interface IConfigLoader
    {
        bool CanLoadFromStorage(string storageType);
        bool CanLoadFromLocation(string location);
        Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
        Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken);
    }
}
