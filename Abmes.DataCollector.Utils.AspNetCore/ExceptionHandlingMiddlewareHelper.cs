using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Abmes.DataCollector.Utils.AspNetCore;

public static class ExceptionHandlingMiddlewareHelper
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true
    };

    public static async Task ReturnUserErrorAsync(HttpContext context, HttpStatusCode httpStatusCode, string? errorMessage)
    {
        var result = JsonSerializer.Serialize(new { error = errorMessage }, _jsonSerializerOptions);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        await context.Response.WriteAsync(result);
    }
}