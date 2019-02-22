using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public class DestinationConfig
    {
        public string DestinationId { get; }
        public string DestinationType { get; }
        public string LoginName { get; }
        public string LoginSecret { get; }
        public string Root { get; }

        public string CollectPostEndpoint { get; }
        public string GarbageCollectFilePostEndpoint { get; }
        public string FileNamesGetEndpoint { get; }

        public string IdentityServiceUrl { get; }
        public string IdentityServiceClientId { get; }
        public string IdentityServiceClientSecret { get; }
        public string IdentityServiceScope { get; }

        // constructor needed for json deserialization
        public DestinationConfig(string destinationId, string destinationType, string loginName, string loginSecret, string root,
            string collectPostEndpoint, string garbageCollectFilePostEndpoint, string fileNamesGetEndpoint,
            string identityServiceTokenEndpoint, string identityServiceClientId, string identityServiceClientSecret, string identityServiceScope)
        {
            DestinationId = destinationId;
            DestinationType = destinationType;
            LoginName = loginName;
            LoginSecret = loginSecret;
            Root = root;

            CollectPostEndpoint = collectPostEndpoint;
            GarbageCollectFilePostEndpoint = garbageCollectFilePostEndpoint;
            FileNamesGetEndpoint = fileNamesGetEndpoint;

            IdentityServiceUrl = identityServiceTokenEndpoint;
            IdentityServiceClientId = identityServiceClientId;
            IdentityServiceClientSecret = identityServiceClientSecret;
            IdentityServiceScope = identityServiceScope;
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