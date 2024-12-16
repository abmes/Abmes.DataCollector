using System.Text.RegularExpressions;

namespace Abmes.Utils;

public static class FileMaskUtils
{
    public static bool FileNameMatchesFilter(string fileName, string filter)
    {
        return FileNameMatchesFilterRegex(fileName, FilterToRegex(filter));
    }

    public static IEnumerable<string> FileNamesMatchingFilter(IEnumerable<string> fileNames, string filter)
    {
        return FileNamesMatchingFilterRegex(fileNames, FilterToRegex(filter));
    }

    public static IEnumerable<string> FileNamesMatchingFilterRegex(IEnumerable<string> fileNames, IEnumerable<Regex> filterRegex)
    {
        return fileNames.Where(x => FileNameMatchesFilterRegex(x, filterRegex));
    }

    private static bool FileNameMatchesFilterRegex(string fileName, IEnumerable<Regex> filterRegex)
    {
        return filterRegex.Any(x => x.Match(fileName).Value == fileName);
    }

    private static string FilterToRegexString(string filter)
    {
        var parts = filter.Select(c =>
            c switch
            {
                '*' or '?' => "." + c,
                '+' or '(' or ')' or '^' or '$' or '.' or '{' or '}' or '[' or ']' or '|' or '\\' => "\\" + c,
                _ => char.ToLower(c).ToString()
            });

        return string.Join(string.Empty, parts);
    }

    private static IEnumerable<string> FilterToRegexStrings(string filter)
    {
        return filter.Split(',', ';').Select(x => FilterToRegexString(x));
    }

    private static IEnumerable<Regex> FilterToRegex(string filter)
    {
        return FilterToRegexStrings(filter).Select(x => new Regex(x, RegexOptions.Compiled | RegexOptions.IgnoreCase));
    }
}
