using System;
using System.Threading.Tasks;
using Kontecg.Threading.Timers;

namespace Kontecg.Threading.BackgroundWorkers
{
    public abstract class AsyncPeriodicBackgroundWorkerBase : BackgroundWorkerBase
    {
        protected readonly KontecgAsyncTimer Timer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PeriodicBackgroundWorkerBase" /> class.
        /// </summary>
        /// <param name="timer">A timer.</param>
        protected AsyncPeriodicBackgroundWorkerBase(KontecgAsyncTimer timer)
        {
            Timer = timer;
            Timer.Elapsed += Timer_ElapsedAsync;
        }

        public override void Start()
        {
            base.Start();
            Timer.Start();
        }

        public override void Stop()
        {
            Timer.Stop();
            base.Stop();
        }

        public override void WaitToStop()
        {
            Timer.WaitToStop();
            base.WaitToStop();
        }

        /// <summary>
        ///     Periodic works should be done by implementing this method.
        /// </summary>
        protected abstract Task DoWorkAsync();

        /// <summary>
        ///     Handles the Elapsed event of the Timer.
        /// </summary>
        private async Task Timer_ElapsedAsync(KontecgAsyncTimer timer)
        {
            try
            {
                await DoWorkAsync();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }
    }
}
