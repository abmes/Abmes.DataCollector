using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Abmes.DataCollector.Collector.Common.Configuration
{
    public enum GarbageCollectionMode { None, Waterfall, Excess }

    public class DestinationConfig
    {
        public string DestinationId { get; set;  }
        public string DestinationType { get; set;  }
        public string LoginName { get; set;  }
        public string LoginSecret { get; set;  }
        public string Root { get; set;  }

        public bool OverrideFiles { get; set;  }
        public bool CollectToDirectories { get; set;  }
        public bool GenerateFileNames { get; set;  }

        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public GarbageCollectionMode? GarbageCollectionMode { get; set;  }

        public string CollectPostEndpoint { get; set;  }
        public string GarbageCollectFilePostEndpoint { get; set;  }
        public string FileNamesGetEndpoint { get; set;  }

        public string IdentityServiceUrl { get; set;  }
        public string IdentityServiceClientId { get; set;  }
        public string IdentityServiceClientSecret { get; set;  }
        public string IdentityServiceScope { get; set;  }


        private IIdentityServiceClientInfo _identityServiceClientInfo;
        public IIdentityServiceClientInfo IdentityServiceClientInfo
        {
            get
            {
                if (_identityServiceClientInfo == null)
                {
                    _identityServiceClientInfo = new IdentityServiceClientInfo(IdentityServiceUrl, IdentityServiceClientId, IdentityServiceClientSecret, IdentityServiceScope, LoginName, LoginSecret);
                }

                return _identityServiceClientInfo;
            }
        }

        public DestinationConfig()
        {
            // needed for deserialization
        }

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