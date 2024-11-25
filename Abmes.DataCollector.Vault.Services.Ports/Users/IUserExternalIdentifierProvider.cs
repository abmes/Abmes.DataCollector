namespace Abmes.DataCollector.Vault.Services.Ports.Users;

public interface IUserExternalIdentifierProvider
{
    Task<string> GetUserExternalIdentifierAsync(CancellationToken cancellationToken);
}
