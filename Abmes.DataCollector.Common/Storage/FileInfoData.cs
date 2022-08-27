using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Common.Storage
{
    public class FileInfoData : IFileInfoData
    {
        public string Name { get; }

        public long? Size { get; }

        [JsonPropertyName("md5")]
        public string MD5 { get; }

        [JsonPropertyName("group")]
        public string GroupId { get; }

        [JsonPropertyName("storage")]
        public string StorageType { get; }

        public FileInfoData(string name, long? size, string md5, string groupId, string storageType)
        {
            Name = name;
            Size = size;
            MD5 = md5;
            GroupId = groupId;
            StorageType = storageType;
        }
    }
}
