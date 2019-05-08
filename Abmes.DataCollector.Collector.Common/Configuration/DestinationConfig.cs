using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public enum GarbageCollectionMode { None, Waterfall, Excess }

    public class DestinationConfig
    {
        public string DestinationId { get; }
        public string DestinationType { get; }
        public string LoginName { get; }
        public string LoginSecret { get; }
        public string Root { get; }

        public bool OverrideFiles { get; }
        public bool CollectToDirectories { get; }
        public bool GenerateFileNames { get; }
        public GarbageCollectionMode? GarbageCollectionMode { get; }

        public string CollectPostEndpoint { get; }
        public string GarbageCollectFilePostEndpoint { get; }
        public string FileNamesGetEndpoint { get; }

        public string IdentityServiceUrl { get; }
        public string IdentityServiceClientId { get; }
        public string IdentityServiceClientSecret { get; }
        public string IdentityServiceScope { get; }

        public IIdentityServiceClientInfo IdentityServiceClientInfo  { get; }

        // constructor needed for json deserialization
        public DestinationConfig(string destinationId, string destinationType, string loginName, string loginSecret, string root, 
            bool overrideFiles, bool collectToDirectories, bool generateFileNames, GarbageCollectionMode? garbageCollectionMode,
            string collectPostEndpoint, string garbageCollectFilePostEndpoint, string fileNamesGetEndpoint,
            string identityServiceUrl, string identityServiceClientId, string identityServiceClientSecret, string identityServiceScope)
        {
            DestinationId = destinationId;
            DestinationType = destinationType;
            LoginName = loginName;
            LoginSecret = loginSecret;
            Root = root;

            OverrideFiles = overrideFiles;
            CollectToDirectories = collectToDirectories;
            GenerateFileNames = generateFileNames;
            GarbageCollectionMode = garbageCollectionMode;

            CollectPostEndpoint = collectPostEndpoint;
            GarbageCollectFilePostEndpoint = garbageCollectFilePostEndpoint;
            FileNamesGetEndpoint = fileNamesGetEndpoint;

            IdentityServiceUrl = identityServiceUrl;
            IdentityServiceClientId = identityServiceClientId;
            IdentityServiceClientSecret = identityServiceClientSecret;
            IdentityServiceScope = identityServiceScope;

            IdentityServiceClientInfo = new IdentityServiceClientInfo(IdentityServiceUrl, IdentityServiceClientId, IdentityServiceClientSecret, IdentityServiceScope, LoginName, LoginSecret);
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