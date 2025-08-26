using System;

namespace Kontecg.Timing.Timezone
{
    /// <summary>
    ///     Interface for timezone converter
    /// </summary>
    public interface ITimeZoneConverter
    {
        /// <summary>
        ///     Converts given date to user's time zone.
        ///     If timezone setting is not specified, returns given date.
        /// </summary>
        /// <param name="date">Base date to convert</param>
        /// <param name="companyId">CompanyId of user</param>
        /// <param name="userId">UserId to convert date for</param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date, int? companyId, long userId);

        /// <summary>
        ///     Converts given date to company's time zone.
        ///     If timezone setting is not specified, returns given date.
        /// </summary>
        /// <param name="date">Base date to convert</param>
        /// <param name="companyId">CompanyId  to convert date for</param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date, int companyId);

        /// <summary>
        ///     Converts given date to application's time zone.
        ///     If timezone setting is not specified, returns given date.
        /// </summary>
        /// <param name="date">Base date to convert</param>
        /// <returns></returns>
        DateTime? Convert(DateTime? date);
    }
}
