﻿using Abmes.Utils;

namespace Abmes.DataCollector.Vault.Web.AspNetCore.Configuration;

public record IdentityServerAuthenticationSettings : IIdentityServerAuthenticationSettings
{
    // todo: .net 7 should support positional record types with non-nullble properties for options
    public string? Authority { get; init; }
    string IIdentityServerAuthenticationSettings.Authority => Ensure.NotNullOrEmpty(Authority);

    public string? ApiName { get; init; }
    string IIdentityServerAuthenticationSettings.ApiName => Ensure.NotNullOrEmpty(ApiName);

    public string? ApiSecret { get; init; }
    string IIdentityServerAuthenticationSettings.ApiSecret => Ensure.NotNullOrEmpty(ApiSecret);
}
