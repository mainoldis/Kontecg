using System;
using System.Threading;
using System.Threading.Tasks;
using Kontecg.Dependency;

namespace Kontecg.Threading.Timers
{
    public class KontecgAsyncTimer : RunnableBase, ITransientDependency
    {
        /// <summary>
        ///     This timer is used to perform the task at specified intervals.
        /// </summary>
        private readonly Timer _taskTimer;

        /// <summary>
        ///     Indicates that whether performing the task or _taskTimer is in sleep mode.
        ///     This field is used to wait executing tasks when stopping Timer.
        /// </summary>
        private volatile bool _performingTasks;

        /// <summary>
        ///     Indicates that whether timer is running or stopped.
        /// </summary>
        private volatile bool _running;

        /// <summary>
        ///     This event is raised periodically according to Period of Timer.
        /// </summary>
        public Func<KontecgAsyncTimer, Task> Elapsed = _ => Task.CompletedTask;

        /// <summary>
        ///     Creates a new Timer.
        /// </summary>
        public KontecgAsyncTimer()
        {
            _taskTimer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        ///     Creates a new Timer.
        /// </summary>
        /// <param name="period">Task period of timer (as milliseconds)</param>
        /// <param name="runOnStart">Indicates whether timer raises Elapsed event on Start method of Timer for once</param>
        public KontecgAsyncTimer(int period, bool runOnStart = false)
            : this()
        {
            Period = period;
            RunOnStart = runOnStart;
        }

        /// <summary>
        ///     Task period of timer (as milliseconds).
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        ///     Indicates whether timer raises Elapsed event on Start method of Timer for once.
        ///     Default: False.
        /// </summary>
        public bool RunOnStart { get; set; }

        /// <summary>
        ///     Starts the timer.
        /// </summary>
        public override void Start()
        {
            if (Period <= 0)
            {
                throw new KontecgException("Period should be set before starting the timer!");
            }

            base.Start();

            _running = true;
            _taskTimer.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
        }

        /// <summary>
        ///     Stops the timer.
        /// </summary>
        public override void Stop()
        {
            lock (_taskTimer)
            {
                _running = false;
                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            base.Stop();
        }

        /// <summary>
        ///     Waits the service to stop.
        /// </summary>
        public override void WaitToStop()
        {
            lock (_taskTimer)
            {
                while (_performingTasks)
                {
                    Monitor.Wait(_taskTimer);
                }
            }

            base.WaitToStop();
        }

        /// <summary>
        ///     This method is called by _taskTimer.
        /// </summary>
        /// <param name="state">Not used argument</param>
        private void TimerCallBack(object state)
        {
            lock (_taskTimer)
            {
                if (!_running || _performingTasks)
                {
                    return;
                }

                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _performingTasks = true;
            }

            _ = Timer_ElapsedAsync();
        }

        private async Task Timer_ElapsedAsync()
        {
            try
            {
                await Elapsed(this);
            }
            catch (Exception)
            {
            }
            finally
            {
                lock (_taskTimer)
                {
                    _performingTasks = false;
                    if (IsRunning)
                    {
                        _taskTimer.Change(Period, Timeout.Infinite);
                    }

                    Monitor.Pulse(_taskTimer);
                }
            }
        }
    }
}
