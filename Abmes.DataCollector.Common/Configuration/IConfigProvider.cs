namespace Abmes.DataCollector.Common.Configuration
{
    public interface IConfigProvider
    {
        Task<string> GetConfigContentAsync(string configName, CancellationToken cancellationToken);
    }
}
