namespace Abmes.DataCollector.Vault.Configuration
{
    public class StorageConfig
    {
        public string StorageType { get; set; }
        public string LoginName { get; set; }
        public string LoginSecret { get; set; }
        public string Root { get; set; }

        public StorageConfig()
        {
            // needed for deserialization
        }

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