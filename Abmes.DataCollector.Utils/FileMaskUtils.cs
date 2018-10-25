using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Abmes.DataCollector.Utils
{
    public static class FileMaskUtils
    {
        public static bool FileNameMatchesFilter(string fileName, string filter)
        {
            var filterRegex = FilterToRegex(filter);
            return FileNameMatchesFilterRegex(fileName, filterRegex);
        }

        public static IEnumerable<string> FileNamesMatchingFilter(IEnumerable<string> fileNames, string filter)
        {
            var filterRegex = FilterToRegex(filter);
            return fileNames.Where(x => FileNameMatchesFilterRegex(x, filterRegex));
        }

        private static bool FileNameMatchesFilterRegex(string fileName, IEnumerable<Regex> filterRegex)
        {
            return filterRegex.Any(x => x.Match(fileName).Value == fileName);
        }

        private static string FilterToRegexString(string filter)
        {
            if (filter == null)
            {
                return null;
            }

            var buffer = new StringBuilder();
            var chars = filter.ToCharArray();

            foreach (var c in chars)
            {
                if (c == '*')
                    buffer.Append(".*");
                else if (c == '?')
                    buffer.Append(".?");
                else if ("+()^$.{}[]|\\".IndexOf(c) != -1)
                    buffer.Append('\\').Append(c); // prefix all metacharacters with backslash
                else
                    buffer.Append(c);
            }

            return buffer.ToString().ToLower();
        }

        private static IEnumerable<string> FilterToRegexStrings(string filter)
        {
            if (filter == null)
            {
                return Enumerable.Empty<string>();
            }

            return filter.Split(',', ';').Select(x => FilterToRegexString(x));
        }

        private static IEnumerable<Regex> FilterToRegex(string filter)
        {
            return FilterToRegexStrings(filter).Select(x => new Regex(x, RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }
    }
}
