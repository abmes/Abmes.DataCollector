using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Storage
{
    public delegate IFileInfo IFileInfoFactory(string name, long? size, string md5);
}
