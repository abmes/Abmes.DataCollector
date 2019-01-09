using System;
using System.Collections.Generic;
using System.Linq;
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

        public string RootBase()
        {
            return Root?.Split('/', '\\').FirstOrDefault();
        }

        public string RootDir(char separator, bool includeTrailingSeparator)
        {
            var result = string.Join(separator, Root?.Split('/', '\\').Skip(1));

            if (includeTrailingSeparator && (!string.IsNullOrEmpty(result)))
            {
                result = result + separator;
            }

            return result;
        }
    }
}