namespace Abmes.DataCollector.Utils.Net;

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
}
