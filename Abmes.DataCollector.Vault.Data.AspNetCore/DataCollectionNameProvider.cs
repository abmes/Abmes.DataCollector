using Abmes.DataCollector.Vault.Services.Ports.Configuration;
using Microsoft.AspNetCore.Http;

namespace Abmes.DataCollector.Vault.Data.AspNetCore;

public class DataCollectionNameProvider(
    IHttpContextAccessor httpContextAccessor)
    : IDataCollectionNameProvider
{
    public string GetDataCollectionName()
    {
        var result = httpContextAccessor.HttpContext?.Request.Headers["DataCollectionName"].ToString();
        ArgumentException.ThrowIfNullOrEmpty(result);
        return result;
    }
}
