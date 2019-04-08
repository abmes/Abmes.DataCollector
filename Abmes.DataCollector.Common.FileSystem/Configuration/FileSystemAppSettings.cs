using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.FileSystem.Configuration
{
    public class FileSystemAppSettings : IFileSystemAppSettings
    {
        public string FileSystemConfigStorageRoot { get; set; }
    }
}
