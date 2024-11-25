namespace Abmes.DataCollector.Vault.Web.AspNetCore.Configuration;

public interface IIdentityServerAuthenticationSettings
{
    string Authority { get; }
    string ApiName { get; }
    string ApiSecret { get; }
}