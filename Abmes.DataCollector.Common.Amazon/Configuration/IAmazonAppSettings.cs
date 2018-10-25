using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Amazon.Configuration
{
    public interface IAmazonAppSettings
    {
        string AmazonS3ConfigStorageBucketName { get; }
    }
}
