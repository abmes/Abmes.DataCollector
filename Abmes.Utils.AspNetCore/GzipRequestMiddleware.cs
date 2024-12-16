using Microsoft.AspNetCore.Http;
using System.IO.Compression;

namespace Abmes.Utils.AspNetCore;

public class GzipRequestMiddleware(
    RequestDelegate next)
{
    private const string ContentEncodingGzip = "gzip";
    private const string ContentEncodingDeflate = "deflate";

    public async Task InvokeAsync(HttpContext context)
    {
        switch (context.Request.Headers.ContentEncoding)
        {
            case ContentEncodingGzip:
                context.Request.Body = new GZipStream(context.Request.Body, CompressionMode.Decompress, true);
                break;

            case ContentEncodingDeflate:
                context.Request.Body = new DeflateStream(context.Request.Body, CompressionMode.Decompress, true);
                break;
        }

        await next(context);
    }
}
