using Abmes.DataCollector.Utils;
using IdentityModel.Client;
using System.Net;
using System.Net.Http.Headers;

namespace Abmes.DataCollector.Collector.Common.Identity;

public class IdentityServiceHttpRequestConfigurator(
    IHttpClientFactory httpClientFactory) : IIdentityServiceHttpRequestConfigurator
{
    public async Task ConfigAsync(HttpRequestMessage request, IdentityServiceClientInfo? identityServiceClientInfo, CancellationToken cancellationToken)
    {
        if (identityServiceClientInfo is not null)
        {
            var accessToken = await GetIdentityServiceAccessTokenAsync(identityServiceClientInfo, cancellationToken);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }

    public void Config(HttpRequestMessage request, string? identityServiceAccessToken, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(identityServiceAccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", identityServiceAccessToken);
        }
    }

    public async Task<string> GetIdentityServiceAccessTokenAsync(IdentityServiceClientInfo identityServiceClientInfo, CancellationToken cancellationToken)
    {
        var tokenRequest = new PasswordTokenRequest
        {
            Address = identityServiceClientInfo.Url.TrimEnd('/') + "/connect/token",

            ClientId = identityServiceClientInfo.ClientId,
            ClientSecret = identityServiceClientInfo.ClientSecret,
            Scope = identityServiceClientInfo.Scope,

            UserName = identityServiceClientInfo.UserName,
            Password = identityServiceClientInfo.UserPassword
        };

        using var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.RequestPasswordTokenAsync(tokenRequest, cancellationToken);

        ArgumentExceptionExtensions.ThrowIf(response.HttpStatusCode is not HttpStatusCode.OK);
        ArgumentNullException.ThrowIfNull(response.AccessToken);

        return response.AccessToken;
    }
}
