using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Storage
{
    public delegate IFileInfoData IFileInfoDataFactory(string name, long? size, string md5, string groupId, string storageType);
}
