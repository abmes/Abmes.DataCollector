namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public class RelativeDateInfo
    {
        private class DateInfo
        {
            public int YearNo { get; }
            public int QuarterNo { get; }
            public int MonthNo { get; }
            public int WeekNo { get; }
            public int DayNo { get; }
            public DateTimeOffset Date { get; }

            public DateInfo(DateTimeOffset date)
            {
                var rootDate = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.FromMilliseconds(0));

                YearNo = date.Year - rootDate.Year + 1;
                QuarterNo = (date.Year - rootDate.Year) * 4 + (int)Math.Ceiling((double)date.Month / 3);
                MonthNo = (date.Year - rootDate.Year) * 12 + date.Month;
                DayNo = (date - rootDate).Days + 1;
                WeekNo = (int)Math.Ceiling((double)DayNo / 7);
            }
        }

        public int RelativeYearNo { get; }
        public int RelativeQuarterNo { get; }
        public int RelativeMonthNo { get; }
        public int RelativeWeekNo { get; }
        public int RelativeDayNo { get; }
        public DateTimeOffset Date { get; }
        public DateTimeOffset RelativeToDate { get; }

        public RelativeDateInfo(DateTimeOffset date, DateTimeOffset relativeToDate)
        {
            Date = date;
            RelativeToDate = relativeToDate;

            var dateInfo = new DateInfo(date);
            var relativeToDateInfo = new DateInfo(relativeToDate);

            RelativeYearNo = relativeToDateInfo.YearNo - dateInfo.YearNo;
            RelativeQuarterNo = relativeToDateInfo.QuarterNo - dateInfo.QuarterNo;
            RelativeMonthNo = relativeToDateInfo.MonthNo - dateInfo.MonthNo;
            RelativeWeekNo = relativeToDateInfo.WeekNo - dateInfo.WeekNo;
            RelativeDayNo = relativeToDateInfo.DayNo - dateInfo.DayNo;
        }
    }
}
