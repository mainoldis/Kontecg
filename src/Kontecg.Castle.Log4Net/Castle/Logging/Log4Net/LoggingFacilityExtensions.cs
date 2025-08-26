using Castle.Facilities.Logging;

namespace Kontecg.Castle.Logging.Log4Net
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseKontecgLog4Net(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<Log4NetLoggerFactory>();
        }
    }
}
