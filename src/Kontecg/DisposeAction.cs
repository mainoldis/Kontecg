using System;
using System.Threading;
using JetBrains.Annotations;

namespace Kontecg
{
    /// <summary>
    ///     This class can be used to provide an action when
    ///     Dispose method is called.
    /// </summary>
    public class DisposeAction : IDisposable
    {
        public static readonly DisposeAction Empty = new(null);

        private Action _action;

        /// <summary>
        ///     Creates a new <see cref="DisposeAction" /> object.
        /// </summary>
        /// <param name="action">Action to be executed when this object is disposed.</param>
        public DisposeAction([CanBeNull] Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            // Interlocked prevents multiple execution of the _action.
            Action action = Interlocked.Exchange(ref _action, null);
            action?.Invoke();
        }
    }
}
