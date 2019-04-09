using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Storage
{
    public class FileInfo : IFileInfo
    {
        public string Name { get; }

        public long? Size { get; }

        [JsonProperty(PropertyName = "md5")]
        public string MD5 { get; }

        public FileInfo(string name, long? size, string md5)
        {
            Name = name;
            Size = size;
            MD5 = md5;
        }
    }
}
