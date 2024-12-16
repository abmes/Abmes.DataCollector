namespace Abmes.Utils.AspNetCore.Cdn;

public class Cdnizer<T>(
    ICdnSettings cdnSettings)
    : ICdnizer<T>
    where T : class
{
    public string this[string relativePath] => Cdnize(Librarize(relativePath));

    private static string Librarize(string relativePath)
    {
        return $"/_content/{typeof(T).Assembly.GetName().Name}/{relativePath.TrimStart('/')}";
    }

    private string Cdnize(string relativePath)
    {
        return
            string.IsNullOrEmpty(cdnSettings.CdnUrl)
            ? relativePath
            : new Uri(new Uri(cdnSettings.CdnUrl), relativePath).ToString();
    }
}
