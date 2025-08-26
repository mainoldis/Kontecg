namespace Kontecg.Timing
{
    public static class ClockProviders
    {
        public static UnspecifiedClockProvider Unspecified { get; } = new();

        public static LocalClockProvider Local { get; } = new();

        public static UtcClockProvider Utc { get; } = new();
    }
}
