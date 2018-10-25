using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Configuration
{
    public class StorageConfig
    {
        public string StorageType { get; }
        public string LoginName { get; }
        public string LoginSecret { get; }
        public string Root { get; }

        // constructor needed for json deserialization
        public StorageConfig(string storageType, string loginName, string loginSecret, string root)
        {
            StorageType = storageType;
            LoginName = loginName;
            LoginSecret = loginSecret;
            Root = root;
        }
    }
}