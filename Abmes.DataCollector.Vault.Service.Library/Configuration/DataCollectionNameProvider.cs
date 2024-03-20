using Abmes.DataCollector.Vault.Configuration;
using Microsoft.AspNetCore.Http;

namespace Abmes.DataCollector.Vault.WebAPI.Configuration;

public class DataCollectionNameProvider(
    IHttpContextAccessor httpContextAccessor) : IDataCollectionNameProvider
{
    public string GetDataCollectionName()
    {
        var result = httpContextAccessor.HttpContext?.Request.Headers["DataCollectionName"].ToString();
        ArgumentException.ThrowIfNullOrEmpty(result);
        return result;
    }
}
