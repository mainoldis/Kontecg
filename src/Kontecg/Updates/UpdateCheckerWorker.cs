using System;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Extensions;
using Kontecg.Threading.BackgroundWorkers;
using Kontecg.Threading.Timers;
using Kontecg.Timing;

namespace Kontecg.Updates
{
    public class UpdateCheckerWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IUpdateManager _updateManager;

        public UpdateCheckerWorker(
            KontecgAsyncTimer timer,
            IUpdateConfiguration updateConfiguration,
            IUpdateManager updateManager)
            : base(timer)
        {
            _updateManager = updateManager;
            Timer.Period = updateConfiguration.CheckUpdatePeriod?.TotalMilliseconds.To<int>() ??
                           TimeSpan.FromMinutes(1).TotalMilliseconds.To<int>();
        }

        protected override async Task DoWorkAsync()
        {
            DateTime utcNow = Clock.Now;
            Logger.InfoFormat("Check for outdated plugins at {0}", utcNow);

            bool updateAsync = await _updateManager.CheckForUpdateAsync();
        }
    }
}
