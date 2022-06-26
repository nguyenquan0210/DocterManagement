using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.ViewModels.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUnixTimeStamp(this long unixTimeStamp)
        {
            var timeSpan = TimeSpan.FromSeconds(unixTimeStamp);
            return new DateTime(timeSpan.Ticks + new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).UtcToLocalTime().Ticks);
        }

        public static DateTime? FromUnixTimeStamp(this long? unixTimeStamp)
        {
            return unixTimeStamp >= -6847786800 ? FromUnixTimeStamp(unixTimeStamp.Value) : (DateTime?)null;
        }

        public static long? ToSecondsTimestamp(this DateTime? date)
        {
            if (date == null)
                return null;

            return ToSecondsTimestamp((DateTime)date);
        }

        public static long? ToSecondsTimestamp(this DateTime date)
        {
            if (date.Year == 9999)
            {
                return null;
            }
            var span = date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).UtcToLocalTime());
            return (long)span.TotalSeconds;
        }

        public static double DateDiff(this DateTime fromDate, DateTime toDate)
        {
            TimeSpan ts = toDate - fromDate;
            return ts.TotalDays;
        }

        public static DateTime LocalToUtcTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, TimeZoneInfo.Local.Id, TimeZoneInfo.Utc.Id);
        }

        public static DateTime UtcToLocalTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, TimeZoneInfo.Utc.Id, TimeZoneInfo.Local.Id);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static string ToString(this DateTime? date, string format)
        {
            if (date == null)
                return string.Empty;
            return format.IsNullOrWhiteSpace() ? ((DateTime)date).ToString() : ((DateTime)date).ToString(format);
        }
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        public static DateTime GetEndDateOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }
    }
}
