namespace Abmes.DataCollector.Vault.Services.Contracts;

public interface IUserService
{
    Task<bool> IsUserAllowedDataCollectionAsync(CancellationToken cancellationToken);
}
