using System;

namespace Kontecg.Timing
{
    /// <summary>
    ///     Implements <see cref="IClockProvider" /> to work with local times.
    /// </summary>
    public class LocalClockProvider : IClockProvider
    {
        internal LocalClockProvider()
        {
        }

        public DateTime Now => DateTime.Now;

        public DateTimeKind Kind => DateTimeKind.Local;

        public bool SupportsMultipleTimezone => false;

        public DateTime Normalize(DateTime dateTime)
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Local),
                DateTimeKind.Utc => dateTime.ToLocalTime(),
                _ => dateTime
            };
        }
    }
}
