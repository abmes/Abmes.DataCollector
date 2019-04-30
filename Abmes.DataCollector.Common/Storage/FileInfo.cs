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

        [JsonProperty(PropertyName = "group")]
        public string GroupId { get; }

        [JsonProperty(PropertyName = "storage")]
        public string StorageType { get; }

        public FileInfo(string name, long? size, string md5, string groupId, string storageType)
        {
            Name = name;
            Size = size;
            MD5 = md5;
            GroupId = groupId;
            StorageType = storageType;
        }
    }
}
