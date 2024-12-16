namespace Abmes.Utils.Net;

public static class HttpUtils
{
    public static bool IsUrl(string url)
    {
        return
            !string.IsNullOrEmpty(url) &&
            (url.StartsWith("http://") || url.StartsWith("https://"));
    }

    public static IEnumerable<KeyValuePair<string, string>> GetHeaders(string headers)
    {
        return
            headers
                .Split(';', ',')
                .Select(x => { var parts = x.Split('='); return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim()); });
    }

    public static string FixJsonResult(string json)
    {
        var s = json.Trim('"').Trim();

        while (!string.IsNullOrEmpty(s) && (s.EndsWith(@"\n") || s.EndsWith(@"\r")))
        {
            s = s.Remove(s.Length - 2);
        }

        return s;
    }
}
