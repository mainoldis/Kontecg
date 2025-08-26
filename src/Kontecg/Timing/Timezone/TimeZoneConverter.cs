using System;
using Kontecg.Configuration;
using Kontecg.Dependency;

namespace Kontecg.Timing.Timezone
{
    /// <summary>
    ///     Time zone converter class
    /// </summary>
    public class TimeZoneConverter : ITimeZoneConverter, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="settingManager"></param>
        public TimeZoneConverter(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <inheritdoc />
        public DateTime? Convert(DateTime? date, int? companyId, long userId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!Clock.SupportsMultipleTimezone)
            {
                return date;
            }

            string usersTimezone =
                _settingManager.GetSettingValueForUser(TimingSettingNames.TimeZone, companyId, userId);
            return string.IsNullOrEmpty(usersTimezone)
                ? date
                : TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), usersTimezone);
        }

        /// <inheritdoc />
        public DateTime? Convert(DateTime? date, int companyId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!Clock.SupportsMultipleTimezone)
            {
                return date;
            }

            string companiesTimezone =
                _settingManager.GetSettingValueForCompany(TimingSettingNames.TimeZone, companyId);
            return string.IsNullOrEmpty(companiesTimezone)
                ? date
                : TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), companiesTimezone);
        }

        /// <inheritdoc />
        public DateTime? Convert(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            if (!Clock.SupportsMultipleTimezone)
            {
                return date;
            }

            string applicationsTimezone = _settingManager.GetSettingValueForApplication(TimingSettingNames.TimeZone);
            return string.IsNullOrEmpty(applicationsTimezone)
                ? date
                : TimezoneHelper.ConvertFromUtc(date.Value.ToUniversalTime(), applicationsTimezone);
        }
    }
}
