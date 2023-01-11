using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Abmes.DataCollector.Utils.AspNetCore;

public static class ExceptionHandlingMiddlewareHelper
{
    public static async Task ReturnUserErrorAsync(HttpContext context, HttpStatusCode httpStatusCode, string? errorMessage)
    {
        var options = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Serialize(new { error = errorMessage }, options);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        await context.Response.WriteAsync(result);
    }
}