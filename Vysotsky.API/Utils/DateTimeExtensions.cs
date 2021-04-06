using System;

namespace Vysotsky.API.Utils
{
    internal static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var span = dateTime - epoch;
            return (long) span.TotalMilliseconds;
        }

        public static DateTime ToDateTime(this long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(timestamp);
        }
    }
}
