using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Azure.Configuration
{
    public interface IAzureAppSettings
    {
        string AzureConfigStorageContainerName { get; }
    }
}
