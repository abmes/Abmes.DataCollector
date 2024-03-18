using Abmes.DataCollector.Utils;
using Abmes.DataCollector.Vault.Configuration;
using Microsoft.AspNetCore.Http;

namespace Abmes.DataCollector.Vault.WebAPI.Configuration;

public class DataCollectionNameProvider : IDataCollectionNameProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataCollectionNameProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetDataCollectionName()
    {
        var result = _httpContextAccessor.HttpContext?.Request.Headers["DataCollectionName"].ToString();
        ArgumentExceptionExtensions.ThrowIfNullOrEmpty(result);
        return result;
    }
}
