﻿using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Linq;

namespace Abmes.DataCollector.Collector.Common.Misc;

public class DateTimeFormatter : IDateTimeFormatter
{
    [return: NotNullIfNotNull("format")]
    public string? FormatDateTime(string? format, string patternPrefix, string patternSuffix, DateTime dateTime)
    {
        Contract.Requires(!(string.IsNullOrEmpty(patternPrefix)));
        Contract.Requires(!(string.IsNullOrEmpty(patternSuffix)));

        if (string.IsNullOrEmpty(format))
        {
            return null;
        }

        var pattern = patternPrefix + "([a-zA-Z]+)" + patternSuffix;
        var matches = Regex.Matches(format, pattern);

        var result = format;
        foreach (var match in matches.Where(match => match.Groups.Count == 2))
        {
            var dateTimeFormat = match.Groups[1].Value;
            result = result.Replace(match.Value, dateTime.ToString(dateTimeFormat));
        }

        return result;
    }
}
