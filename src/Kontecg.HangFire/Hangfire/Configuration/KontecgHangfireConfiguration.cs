using Hangfire;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;

namespace Kontecg.Hangfire.Configuration
{
    public class KontecgHangfireConfiguration : IKontecgHangfireConfiguration
    {
        public BackgroundJobServer Server { get; set; }

        public IGlobalConfiguration GlobalConfiguration
        {
            get { return HangfireGlobalConfiguration.Configuration; }
        }
    }
}