using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Storage
{
    public interface IFileNameProvider
    {
        string GenerateCollectDestinationFileName(string dataCollectionName, string collectUrl, DateTimeOffset collectMoment, bool collectToDirectories, bool generateFileNames);
        DateTimeOffset DataCollectionFileNameToDateTime(string fileName);
    }
}
