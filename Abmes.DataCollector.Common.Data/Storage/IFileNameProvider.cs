﻿namespace Abmes.DataCollector.Common.Data.Storage;

public interface IFileNameProvider
{
    string LockFileName { get; }
    string GenerateCollectDestinationFileName(string dataCollectionName, string? collectItemName, string collectUrl, DateTimeOffset collectMoment, bool collectToDirectories, bool generateFileNames);
    DateTimeOffset DataCollectionFileNameToDateTime(string fileName);
}
