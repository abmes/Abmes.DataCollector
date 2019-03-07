using Abmes.DataCollector.Collector.Common.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Service.Configuration
{
    public class DataCollectionsFilterProvider : IDataCollectionsFilterProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataCollectionsFilterProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetDataCollectionsFilterAsync(CancellationToken cancellationToken)
        {
            var result = _httpContextAccessor.HttpContext.Request.Query["dataCollections"].ToString();

            return await Task.FromResult(result);
        }
    }
}
