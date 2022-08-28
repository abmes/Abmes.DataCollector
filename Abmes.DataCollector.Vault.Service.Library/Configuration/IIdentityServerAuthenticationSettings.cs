﻿namespace Abmes.DataCollector.Vault.Service.Configuration;

public interface IIdentityServerAuthenticationSettings
{
    string Authority { get; }
    string ApiName { get; }
    string ApiSecret { get; }
}