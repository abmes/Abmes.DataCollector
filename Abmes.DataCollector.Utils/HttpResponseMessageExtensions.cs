namespace Abmes.DataCollector.Utils;

public static class HttpResponseMessageExtensions
{
    public static async Task CheckSuccessAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"{response.ReasonPhrase} {(int)response.StatusCode}" + Environment.NewLine + errorMessage);
        }
    }

    public static string? ContentMD5(this HttpResponseMessage response)
    {
        return
            CopyUtils.GetMD5HashString(response.Content.Headers.ContentMD5)
            ??
            response.Headers.Where(x => x.Key.Equals("x-amz-meta-content-md5", StringComparison.InvariantCultureIgnoreCase)).Select(z => z.Value.FirstOrDefault()).FirstOrDefault();
    }
}
