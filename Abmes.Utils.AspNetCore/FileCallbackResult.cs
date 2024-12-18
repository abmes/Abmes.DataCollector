using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Abmes.Utils.AspNetCore;

// copied from http://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html
public class FileCallbackResult(
    MediaTypeHeaderValue contentType,
    Func<Stream, ActionContext, Task> callback)
    : FileResult(contentType.ToString())
{
    private readonly Func<Stream, ActionContext, Task> _callback = callback;  // so that we can use it in the executor

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var executor = new FileCallbackResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
        await executor.ExecuteAsync(context, this);
    }

    private sealed class FileCallbackResultExecutor(
        ILoggerFactory loggerFactory)
        : FileResultExecutorBase(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
    {
        public async Task ExecuteAsync(ActionContext context, FileCallbackResult result)
        {
            SetHeadersAndLog(context, result, null, false);
            await result._callback(context.HttpContext.Response.Body, context);
        }
    }
}