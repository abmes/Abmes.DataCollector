using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Amazon.Configuration
{
    public class AmazonAppSettings : IAmazonAppSettings
    {
        public string AmazonS3ConfigStorageBucketName { get; set; }
    }
}
