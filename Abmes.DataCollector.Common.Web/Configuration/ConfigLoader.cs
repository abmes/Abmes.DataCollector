using Abmes.DataCollector.Common.Configuration;
using Abmes.DataCollector.Utils;

namespace Abmes.DataCollector.Common.Web.Configuration
{
    public class ConfigLoader : IConfigLoader
    {
        private const string HttpsPrefix = "https://";

        private readonly IHttpClientFactory _httpClientFactory;

        public ConfigLoader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public bool CanLoadFromLocation(string location)
        {
            return
                location.StartsWith(HttpsPrefix, StringComparison.InvariantCultureIgnoreCase) &&
                !location.Equals(HttpsPrefix, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanLoadFromStorage(string storageType)
        {
            return false;
        }

        public Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetConfigContentAsync(string configName, string location, CancellationToken cancellationToken)
        {
            var bracketPos = location.IndexOf("[");
            var headers = (bracketPos >= 0) ? GetHeaders(location.Substring(bracketPos)) : null;
            var url = location.Substring(0, bracketPos).TrimEnd('/') + "/" + configName;

            using var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.GetStringAsync(url, headers, null, null, cancellationToken);
        }

        private IEnumerable<KeyValuePair<string, string>> GetHeaders(string headers)
        {
            return HttpUtils.GetHeaders(headers.Trim('[', ']'));
        }
    }
}
