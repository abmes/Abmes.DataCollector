﻿using Abmes.DataCollector.Collector.Services.Ports.Identity;

namespace Abmes.DataCollector.Collector.Services.Ports.Destinations;

public interface IDestination
{
    DestinationConfig DestinationConfig { get; }
    bool CanGarbageCollect();
    Task PutFileAsync(string dataCollectionName, string fileName, Stream content, CancellationToken cancellationToken);
    Task CollectAsync(string collectUrl, IEnumerable<KeyValuePair<string, string>> collectHeaders, IdentityServiceClientInfo? collectIdentityServiceClientInfo, string dataCollectionName, string fileName, TimeSpan timeout, bool finishWait, CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetDataCollectionFileNamesAsync(string dataCollectionName, CancellationToken cancellationToken);
    Task GarbageCollectDataCollectionFileAsync(string dataCollectionName, string fileName, CancellationToken cancellationToken);
    Task<bool> AcceptsFileAsync(string dataCollectionName, string name, long? size, string? md5, CancellationToken cancellationToken);
}
