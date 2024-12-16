using Abmes.Utils.UserExceptions;

namespace Abmes.Utils.Net;

public static class HttpResponseMessageExtensions
{
    public static async Task CheckSuccessAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);

            string? userExceptionMessage;
            try
            {
                userExceptionMessage = System.Text.Json.JsonDocument.Parse(errorMessage).RootElement.GetProperty("error").GetString();
            }
            catch (System.Text.Json.JsonException)
            {
                userExceptionMessage = null;
            }

            if (!string.IsNullOrEmpty(userExceptionMessage))
            {
                throw new UserException(userExceptionMessage);
            }
            else
            {
                throw new HttpRequestException($"{response.ReasonPhrase} {(int)response.StatusCode}" + Environment.NewLine + errorMessage);
            }
        }
    }
}
