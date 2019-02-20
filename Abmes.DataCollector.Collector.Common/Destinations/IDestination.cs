﻿using Abmes.DataCollector.Collector.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Destinations
{
    public interface IDestination
    {
        DestinationConfig DestinationConfig { get; }  
        Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, int tryNo, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken);
        Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken);
    }
}
