namespace Abmes.DataCollector.Vault.Web.Library.Configuration;

public interface IIdentityServerAuthenticationSettings
{
    string Authority { get; }
    string ApiName { get; }
    string ApiSecret { get; }
}