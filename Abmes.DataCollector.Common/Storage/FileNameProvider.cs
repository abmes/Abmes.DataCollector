using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Abmes.DataCollector.Common.Storage
{
    public class FileNameProvider : IFileNameProvider
    {
        private const string SDateFormat = "yyyy-MM-dd-HHmmss";

        public string LockFileName => "datacollector.lock";

        public string GenerateCollectDestinationFileName(string dataCollectionName, string collectItemName, string collectUrl, DateTimeOffset collectMoment, bool collectToDirectories, bool generateFileNames)
        {
            var time = collectMoment.ToUniversalTime().ToString(SDateFormat);

            var fileName = new Uri(collectUrl).LocalPath.Split("/").Last();

            if (generateFileNames)
            {
                var ext = System.IO.Path.GetExtension(fileName);
                fileName = $"{dataCollectionName}-{time}{ext}";
            }

            if (collectToDirectories)
            {
                fileName = $"{dataCollectionName}-{time}/{fileName}";
            }
            else
            {
                if (!string.IsNullOrEmpty(collectItemName))
                {
                    fileName = string.Join("/", collectItemName.Split("/").SkipLast(1).Concat(new[] { fileName }));
                }
            }

            return fileName;
        }

        public DateTimeOffset DataCollectionFileNameToDateTime(string fileName)
        {
            var dateTimeString = ExtractDateTimeString(fileName);

            return DateTimeOffset.ParseExact(dateTimeString, SDateFormat, CultureInfo.InvariantCulture);
        }

        private static string ExtractDateTimeString(string fileName)
        {
            var nameParts = fileName.Replace(@"\", "/").Split("/");

            if (nameParts.Count() > 1)
            {
                return ExtractDateTimeString(nameParts[0]);
            }

            try
            {
                var dateTimeParts = System.IO.Path.GetFileNameWithoutExtension(fileName).Split('-').Reverse().Take(4).Reverse();

                var dateString = string.Join("-", dateTimeParts.Take(3));

                var timeString = dateTimeParts.Skip(3).First().PadRight(6, '0');

                return dateString + '-' + timeString;
            }
            catch (Exception e)
            {
                throw new Exception($"Could not extract DateTime from file name '{fileName}'", e);
            }
        }
    }
}
