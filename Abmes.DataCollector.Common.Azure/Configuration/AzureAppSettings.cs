using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Azure.Configuration
{
    public class AzureAppSettings : IAzureAppSettings
    {
        public string AzureConfigStorageContainerName { get; set; }
    }
}
