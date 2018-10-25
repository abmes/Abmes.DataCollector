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

        public string GenerateCollectDestinationFileName(string dataCollectionName, string collectUrl, DateTimeOffset collectMoment, bool preserveFileName)
        {
            var u = new Uri(collectUrl);
            var fileName = u.LocalPath.Split("/").Last();

            var time = collectMoment.ToUniversalTime().ToString(SDateFormat);

            if (preserveFileName)
            {
                return time + "/" + fileName;
            }
            else
            {
                var ext = System.IO.Path.GetExtension(fileName);
                return $"{dataCollectionName}-{time}{ext}";
            }
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
                return nameParts[0];
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
