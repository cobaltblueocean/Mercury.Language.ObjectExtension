using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// The number of seconds in one day.
        /// </summary>
        public static long SECONDS_PER_DAY = 86400L;

        /// <summary>
        /// The number of days in one year (estimated as 365.25).
        /// </summary>
        //TODO change this to 365.2425 to be consistent with JSR-310
        public static double DAYS_PER_YEAR = 365.25;

        /// <summary>
        /// The number of milliseconds in one day.
        /// </summary>
        public static long MILLISECONDS_PER_DAY = SECONDS_PER_DAY * 1000;

        /// <summary>
        /// The number of seconds in one year.
        /// </summary>
        public static long SECONDS_PER_YEAR = (long)(SECONDS_PER_DAY * DAYS_PER_YEAR);

        /// <summary>
        /// The number of milliseconds in one year.
        /// </summary>
        public static long MILLISECONDS_PER_YEAR = SECONDS_PER_YEAR * 1000;

        /// <summary>
        /// The number of milliseconds in one month.
        /// </summary>
        public static long MILLISECONDS_PER_MONTH = MILLISECONDS_PER_YEAR / 12L;

        /// <summary>
        /// Check if the DateTime is after the target DateTime
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="target">DateTime to target to find out</param>
        /// <returns>True if the evaluating DateTime is after the target Datetime, otherwise, False</returns>
        public static Boolean IsAfter(this DateTime now, DateTime target)
        {
            if (DateTime.Compare(now, target) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the DateTime is before the target DateTime
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="target">DateTime to target to find out</param>
        /// <returns>True if the evaluating DateTime is before the target Datetime, otherwise, False</returns>
        public static Boolean IsBefore(this DateTime now, DateTime target)
        {
            if (DateTime.Compare(now, target) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the DateTime is after the target DateTime
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="target">DateTime to target to find out</param>
        /// <returns>True if the evaluating DateTime is after the target Datetime, otherwise, False</returns>
        public static Boolean IsAfter(this DateTime? now, DateTime target)
        {
            if (now.HasValue)
                return now.Value.IsAfter(target);
            else
                return false;
        }

        /// <summary>
        /// Check if the DateTime is before the target DateTime
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="target">DateTime to target to find out</param>
        /// <returns>True if the evaluating DateTime is before the target Datetime, otherwise, False</returns>
        public static Boolean IsBefore(this DateTime? now, DateTime target)
        {
            if (now.HasValue)
                return now.Value.IsBefore(target);
            else
                return false;
        }

        /// <summary>
        /// Assign the passed DateTime if the evaluating DateTime is null
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="val">DateTime to assign if evaluating DateTime is null</param>
        /// <returns>Return the evaluating DateTime if not null, otherwise assign the val and return it</returns>
        public static DateTime? FirstNonNull(this DateTime? source, DateTime val)
        {
            if (source == null)
                source = val;

            return source;
        }

        /// <summary>
        /// Add Nanoseconds to the target DateTime
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="nanos">Nanoseconds value to add</param>
        /// <returns>Return the evaluating DateTime which added the Nanoseconds</returns>
        public static DateTime AddNanos(this DateTime source, long nanos)
        {
            return source.AddTicks(nanos * 100);
        }

        /// <summary>
        /// Add Nanoseconds to the target DateTime
        /// </summary>
        /// <param name="now">DateTime to evaluate</param>
        /// <param name="nanos">Nanoseconds value to add</param>
        /// <returns>Return the evaluating DateTime which added the Nanoseconds</returns>
        public static DateTime AddNanos(this DateTime source, double nanos)
        {
            return AddNanos(source, Convert.ToInt64(nanos));
        }

        /// <summary>
        /// Get long value of the DateTime
        /// </summary>
        /// <param name="d">DateTime to get the long value</param>
        /// <returns>Long value of the DateTime</returns>
        public static Int64 GetTime(this DateTime d)
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (d - st);
            retval = (Int64)(t.TotalMilliseconds + 0.5);
            return retval;
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// This is an equivalent method with System.currentTimeMillis() in Java
        /// </summary>
        /// <param name="dt">current DateTime</param>
        /// <returns>The date and time value in UnixTimeMIlliseconds</returns>
        public static long CurrentTimeMillis(this DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - Jan1st1970).TotalMilliseconds;
        }
    }
}
