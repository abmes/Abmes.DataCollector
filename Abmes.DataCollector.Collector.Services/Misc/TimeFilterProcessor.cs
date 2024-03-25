namespace Abmes.DataCollector.Collector.Services.Misc;

public class TimeFilterProcessor : ITimeFilterProcessor
{
    public bool TimeFilterAccepted(string timeFilter)
    {
        if (string.IsNullOrEmpty(timeFilter))
        {
            return true;
        }

        var timeFilters = timeFilter.Split(';');

        var moment = DateTimeOffset.Now;

        return timeFilters.Any(x => SingleTimeFilterAccepted(x, moment));
    }

    private static bool SingleTimeFilterAccepted(string timeFilter, DateTimeOffset moment)
    {
        var timeFilterParts = timeFilter.Split(':');

        if ((timeFilterParts.Length == 0) || (timeFilterParts.Length > 3))
        {
            InvalidTimeFilterError(timeFilter);
        }

        var timeZoneId = (timeFilterParts.Length == 3) ? timeFilterParts[2] : "";
        var timeZoneDateTime = GetTimeZoneDateTime(moment, timeZoneId);

        return 
            TimeInMultiRange(timeZoneDateTime.Hour, timeFilterParts[0]) && 
            TimeInMultiRange(timeZoneDateTime.Minute, timeFilterParts[1]);
    }

    private static bool TimeInMultiRange(int x, string range)
    {
        return
            range.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => TimeInRange(x, r))
            .Any(z => z);
    }

    private static bool TimeInRange(int x, string range)
    {
        if (range == "*")
        {
            return true;
        }

        var rangeParts = range.Split('-');

        if ((rangeParts.Length == 1) &&
            (int.TryParse(rangeParts[0], out var value)))
        {
            return (x == value);
        }

        if ((rangeParts.Length == 2) &&
            (int.TryParse(rangeParts[0], out var start)) &&
            (int.TryParse(rangeParts[1], out var end)))
        {
            return (start <= x) && (x <= end);
        }

        InvalidTimeFilterError("range = " + range);

        return false;
    }

    private static DateTime GetTimeZoneDateTime(DateTimeOffset moment, string timeZoneId)
    {
        if (string.IsNullOrEmpty(timeZoneId))
        {
            return moment.DateTime;
        }

        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return TimeZoneInfo.ConvertTime(moment, timeZoneInfo).DateTime;
    }

    private static void InvalidTimeFilterError(string timeFilter)
    {
        throw new Exception($"Invalid time filter: {timeFilter}");
    }
}
