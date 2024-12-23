﻿using Abmes.Utils;

namespace Abmes.DataCollector.Shared.Data.Amazon.Configuration;

public record AmazonAppSettings : IAmazonAppSettings
{
    // todo: .net 7 should support record types with non-nullble props for options
    // non-nullble properties can be checked like this: https://stackoverflow.com/questions/64784374/c-sharp-9-records-validation
    public string? AmazonS3ConfigStorageBucketName { get; init; }
    public int? AmazonS3ListParallelism { get; init; }

    string IAmazonAppSettings.AmazonS3ConfigStorageBucketName => Ensure.NotNullOrEmpty(AmazonS3ConfigStorageBucketName);
}
