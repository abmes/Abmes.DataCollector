using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Abmes.DataCollector.Utils.AspNetCore;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerFactory _loggerFactory;

    public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerFactory = loggerFactory;
    }

    public async Task InvokeAsync(HttpContext context /* other scoped dependencies */)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _loggerFactory.CreateLogger("Errors").LogError(0, ex, ex.GetAggregateMessages());
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Serialize(new { error = exception.GetAggregateMessages() }, options);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result);
    }
}
