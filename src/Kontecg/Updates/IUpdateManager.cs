using System;
using System.Threading.Tasks;

namespace Kontecg.Updates
{
    /// <summary>
    ///     This interface is used to manage updates system.
    /// </summary>
    public interface IUpdateManager
    {
        /// <summary>
        ///     Fetch the remote store for updates and compare against the current
        ///     version to determine what updates to download.
        /// </summary>
        /// <param name="progress">
        ///     A Observer which can be used to report Progress -
        ///     will return values from 0-100 and Complete, or Throw
        /// </param>
        /// <returns>
        ///     An UpdateInfo object representing the updates to install.
        /// </returns>
        Task<bool> CheckForUpdateAsync(Action<int> progress = null);
    }
}
