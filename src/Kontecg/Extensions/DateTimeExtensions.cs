using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontecg.Extensions
{
    /// <summary>
    /// Provides comprehensive extension methods for the <see cref="DateTime"/> struct to enhance
    /// date and time manipulation capabilities throughout the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// DateTimeExtensions provides a rich set of utility methods that extend the functionality
    /// of the standard <see cref="DateTime"/> struct. These extensions cover common date and
    /// time manipulation tasks and provide consistent, reusable functionality across the application.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Unix Timestamp Conversion:</strong> Convert between DateTime and Unix timestamps</description></item>
    /// <item><description><strong>Date Range Operations:</strong> Start/end of day, week calculations</description></item>
    /// <item><description><strong>Month Operations:</strong> Days of month, week day instances</description></item>
    /// <item><description><strong>DateTime Manipulation:</strong> Trimming milliseconds, timezone handling</description></item>
    /// <item><description><strong>Calendar Operations:</strong> Week calculations, day counting</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Unix Timestamp Support:</strong> Methods for converting between .NET DateTime
    /// and Unix timestamps (seconds since January 1, 1970 UTC) for API integration and
    /// cross-platform compatibility.
    /// </para>
    /// <para>
    /// <strong>Calendar Operations:</strong> Comprehensive support for calendar-based
    /// calculations including week boundaries, month day counting, and recurring date patterns.
    /// </para>
    /// </remarks>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime to a Unix timestamp (seconds since January 1, 1970 UTC).
        /// </summary>
        /// <param name="target">
        /// The DateTime value to convert to a Unix timestamp.
        /// </param>
        /// <returns>
        /// The number of seconds elapsed since January 1, 1970 UTC (Unix epoch).
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts a .NET DateTime value to a Unix timestamp, which represents
        /// the number of seconds that have elapsed since the Unix epoch (January 1, 1970 UTC).
        /// </para>
        /// <para>
        /// <strong>Unix Epoch:</strong> The Unix epoch is January 1, 1970 00:00:00 UTC.
        /// This is the standard reference point for Unix timestamps used across many
        /// programming languages and systems.
        /// </para>
        /// <para>
        /// <strong>Precision:</strong> The method returns the floor value of the total
        /// seconds, effectively truncating any fractional seconds.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for API integration, database storage,
        /// and cross-platform date/time serialization where Unix timestamps are required.
        /// </para>
        /// </remarks>
        public static double ToUnixTimestamp(this DateTime target)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = target - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// Converts a Unix timestamp to a DateTime value.
        /// </summary>
        /// <param name="unixTime">
        /// The Unix timestamp (seconds since January 1, 1970 UTC) to convert.
        /// </param>
        /// <returns>
        /// A DateTime value representing the date and time corresponding to the Unix timestamp.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts a Unix timestamp back to a .NET DateTime value. The Unix
        /// timestamp represents the number of seconds elapsed since the Unix epoch.
        /// </para>
        /// <para>
        /// <strong>Unix Epoch:</strong> The method uses January 1, 1970 00:00:00 UTC as
        /// the reference point for the conversion.
        /// </para>
        /// <para>
        /// <strong>Precision:</strong> The resulting DateTime will have second-level precision.
        /// Any fractional seconds in the Unix timestamp are ignored.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used when receiving Unix timestamps from
        /// external APIs, databases, or other systems that use Unix time format.
        /// </para>
        /// </remarks>
        public static DateTime FromUnixTimestamp(this double unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Gets the end of the day (23:59:59.999) for the specified date.
        /// </summary>
        /// <param name="target">
        /// The DateTime value for which to get the end of day.
        /// </param>
        /// <returns>
        /// A DateTime value representing the end of the day (23:59:59.999) for the specified date.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method returns a DateTime value representing the very end of the day,
        /// which is 23:59:59.999 (one millisecond before midnight of the next day).
        /// </para>
        /// <para>
        /// <strong>Implementation:</strong> The method adds one day to the date and then
        /// subtracts one millisecond to get the last possible moment of the original day.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for date range queries, scheduling
        /// operations, and when you need to represent "the entire day" in time-based
        /// calculations.
        /// </para>
        /// <para>
        /// <strong>Example:</strong> If target is "2023-12-25 14:30:00", the result will
        /// be "2023-12-25 23:59:59.999".
        /// </para>
        /// </remarks>
        public static DateTime ToDayEnd(this DateTime target)
        {
            return target.Date.AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// Gets the first date of the week for the specified date.
        /// </summary>
        /// <param name="dt">
        /// The DateTime value for which to find the start of the week.
        /// </param>
        /// <param name="startOfWeek">
        /// The day of the week that represents the start of the week (e.g., Sunday or Monday).
        /// </param>
        /// <returns>
        /// A DateTime value representing the first date of the week containing the specified date.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method calculates the first day of the week that contains the specified date.
        /// The start of the week is determined by the startOfWeek parameter, allowing for
        /// different cultural conventions (e.g., Sunday vs Monday as the first day of the week).
        /// </para>
        /// <para>
        /// <strong>Calculation:</strong> The method calculates the difference in days between
        /// the target date's day of week and the specified start of week, then subtracts
        /// that number of days to get to the start of the week.
        /// </para>
        /// <para>
        /// <strong>Cultural Flexibility:</strong> By allowing the start of week to be
        /// specified, this method supports different cultural and business conventions
        /// for week boundaries.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for weekly reports, calendar views,
        /// and business logic that operates on week-based time periods.
        /// </para>
        /// </remarks>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Returns all the days of a specified month as an enumerable collection.
        /// </summary>
        /// <param name="year">
        /// The year of the month.
        /// </param>
        /// <param name="month">
        /// The month (1-12).
        /// </param>
        /// <returns>
        /// An enumerable collection of DateTime values representing all days in the specified month.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method generates a collection of DateTime values representing all days
        /// in the specified month. The collection includes every day from the 1st to the
        /// last day of the month.
        /// </para>
        /// <para>
        /// <strong>Implementation:</strong> The method uses <see cref="DateTime.DaysInMonth"/>
        /// to determine the number of days in the month and generates DateTime values
        /// for each day using LINQ.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for calendar views, monthly reports,
        /// and business logic that needs to process all days in a month.
        /// </para>
        /// <para>
        /// <strong>Example:</strong> For February 2024, returns 29 DateTime values
        /// representing February 1-29, 2024.
        /// </para>
        /// </remarks>
        public static IEnumerable<DateTime> DaysOfMonth(int year, int month)
        {
            return Enumerable.Range(0, DateTime.DaysInMonth(year, month))
                .Select(day => new DateTime(year, month, day + 1));
        }

        /// <summary>
        /// Determines the nth instance of a date's day of the week in its month.
        /// </summary>
        /// <param name="dateTime">
        /// The DateTime value for which to find the week day instance.
        /// </param>
        /// <returns>
        /// The nth instance of the day of the week in the month (1-based).
        /// Returns 0 if the date is not found.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method calculates which occurrence of a particular day of the week a date
        /// represents within its month. For example, if a date falls on the third Tuesday
        /// of the month, this method returns 3.
        /// </para>
        /// <para>
        /// <strong>Calculation:</strong> The method generates all days of the month,
        /// filters for the same day of the week as the target date, and finds the position
        /// of the target date within that filtered collection.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for recurring event patterns,
        /// business rules that depend on week day instances (e.g., "third Friday of the month"),
        /// and calendar applications that need to identify specific occurrences.
        /// </para>
        /// <para>
        /// <strong>Example:</strong> For November 29, 2011 (a Tuesday), the method returns 5,
        /// indicating it is the 5th Tuesday of November 2011.
        /// </para>
        /// </remarks>
        public static int WeekDayInstanceOfMonth(this DateTime dateTime)
        {
            int y = 0;
            return DaysOfMonth(dateTime.Year, dateTime.Month)
                .Where(date => dateTime.DayOfWeek.Equals(date.DayOfWeek))
                .Select(x => new {n = ++y, date = x})
                .Where(x => x.date.Equals(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day)))
                .Select(x => x.n).FirstOrDefault();
        }

        /// <summary>
        /// Gets the total number of days in the month of the specified date.
        /// </summary>
        /// <param name="dateTime">
        /// The DateTime value for which to get the total days in the month.
        /// </param>
        /// <returns>
        /// The total number of days in the month.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method returns the total number of days in the month that contains the
        /// specified date. It accounts for leap years and varying month lengths.
        /// </para>
        /// <para>
        /// <strong>Implementation:</strong> The method uses the <see cref="DaysOfMonth"/>
        /// method to generate all days of the month and returns the count of that collection.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for calendar calculations, date
        /// validation, and business logic that needs to know the length of a month.
        /// </para>
        /// <para>
        /// <strong>Examples:</strong>
        /// <list type="bullet">
        /// <item><description>February 2024: 29 days (leap year)</description></item>
        /// <item><description>February 2023: 28 days (non-leap year)</description></item>
        /// <item><description>April 2024: 30 days</description></item>
        /// <item><description>January 2024: 31 days</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static int TotalDaysInMonth(this DateTime dateTime)
        {
            return DaysOfMonth(dateTime.Year, dateTime.Month).Count();
        }

        /// <summary>
        /// Converts any DateTime to an Unspecified DateTimeKind.
        /// </summary>
        /// <param name="date">
        /// The DateTime value to convert.
        /// </param>
        /// <returns>
        /// A new DateTime value with DateTimeKind.Unspecified, preserving the date and time components.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts a DateTime value to have DateTimeKind.Unspecified while
        /// preserving all the date and time components. This is useful when you need to
        /// remove timezone information from a DateTime value.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>If the input DateTime already has DateTimeKind.Unspecified, it is returned unchanged</description></item>
        /// <item><description>If the input DateTime has DateTimeKind.Utc or DateTimeKind.Local, a new DateTime is created with Unspecified kind</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used when storing dates in databases
        /// that don't support timezone information, or when you need to normalize
        /// DateTime values to remove timezone context.
        /// </para>
        /// <para>
        /// <strong>Note:</strong> This operation removes timezone context, so care should
        /// be taken when using this method in timezone-sensitive applications.
        /// </para>
        /// </remarks>
        public static DateTime ToDateTimeUnspecified(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
            {
                return date;
            }

            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Trims the milliseconds component from a DateTime value.
        /// </summary>
        /// <param name="date">
        /// The DateTime value from which to remove milliseconds.
        /// </param>
        /// <returns>
        /// A new DateTime value with milliseconds set to zero, preserving all other components.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a new DateTime value with the milliseconds component set to zero,
        /// while preserving the year, month, day, hour, minute, second, and DateTimeKind.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used when you need to normalize DateTime
        /// values to second-level precision, such as when comparing dates that may have
        /// different millisecond values but represent the same logical moment.
        /// </para>
        /// <para>
        /// <strong>Example:</strong> If the input is "2023-12-25 14:30:45.123", the result
        /// will be "2023-12-25 14:30:45.000".
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> This method creates a new DateTime instance,
        /// so it should be used judiciously in performance-critical scenarios.
        /// </para>
        /// </remarks>
        public static DateTime TrimMilliseconds(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }
    }
}
