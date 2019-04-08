using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.FileSystem.Configuration
{
    public interface IFileSystemAppSettings
    {
        string FileSystemConfigStorageRoot { get; }
    }
}
