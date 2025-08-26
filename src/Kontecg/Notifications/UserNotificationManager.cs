using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Dependency;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Implements  <see cref="IUserNotificationManager" />.
    /// </summary>
    public class UserNotificationManager : IUserNotificationManager, ISingletonDependency
    {
        private readonly INotificationStore _store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotificationManager" /> class.
        /// </summary>
        public UserNotificationManager(INotificationStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Gets the list of notifications for a user asynchronously.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">Optional notification state filter.</param>
        /// <param name="skipCount">Number of notifications to skip.</param>
        /// <param name="maxResultCount">Maximum number of notifications to return.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        /// <returns>List of user notifications.</returns>
        public async Task<List<UserNotification>> GetUserNotificationsAsync(UserIdentifier user,
            UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            List<UserNotificationInfoWithNotificationInfo> userNotifications =
                await _store.GetUserNotificationsWithNotificationsAsync(user, state, skipCount, maxResultCount,
                    startDate, endDate);
            return userNotifications
                .Select(un => un.ToUserNotification())
                .ToList();
        }

        /// <summary>
        /// Gets the list of notifications for a user.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">Optional notification state filter.</param>
        /// <param name="skipCount">Number of notifications to skip.</param>
        /// <param name="maxResultCount">Maximum number of notifications to return.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        /// <returns>List of user notifications.</returns>
        public List<UserNotification> GetUserNotifications(UserIdentifier user, UserNotificationState? state = null,
            int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
        {
            List<UserNotificationInfoWithNotificationInfo> userNotifications =
                _store.GetUserNotificationsWithNotifications(user, state, skipCount, maxResultCount, startDate,
                    endDate);
            return userNotifications
                .Select(un => un.ToUserNotification())
                .ToList();
        }

        /// <summary>
        /// Gets the count of notifications for a user asynchronously.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">Optional notification state filter.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        /// <returns>The count of user notifications.</returns>
        public Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return _store.GetUserNotificationCountAsync(user, state, startDate, endDate);
        }

        /// <summary>
        /// Gets the count of notifications for a user.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">Optional notification state filter.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        /// <returns>The count of user notifications.</returns>
        public int GetUserNotificationCount(UserIdentifier user, UserNotificationState? state = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return _store.GetUserNotificationCount(user, state, startDate, endDate);
        }

        /// <summary>
        /// Gets a specific user notification asynchronously.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userNotificationId">The user notification identifier.</param>
        /// <returns>The user notification, or null if not found.</returns>
        public async Task<UserNotification> GetUserNotificationAsync(int? companyId, Guid userNotificationId)
        {
            UserNotificationInfoWithNotificationInfo userNotification =
                await _store.GetUserNotificationWithNotificationOrNullAsync(companyId, userNotificationId);

            return userNotification?.ToUserNotification();
        }

        /// <summary>
        /// Gets a specific user notification.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userNotificationId">The user notification identifier.</param>
        /// <returns>The user notification, or null if not found.</returns>
        public UserNotification GetUserNotification(int? companyId, Guid userNotificationId)
        {
            UserNotificationInfoWithNotificationInfo userNotification =
                _store.GetUserNotificationWithNotificationOrNull(companyId, userNotificationId);

            return userNotification?.ToUserNotification();
        }

        /// <summary>
        /// Updates the state of a user notification asynchronously.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userNotificationId">The user notification identifier.</param>
        /// <param name="state">The new notification state.</param>
        public Task UpdateUserNotificationStateAsync(int? companyId, Guid userNotificationId,
            UserNotificationState state)
        {
            return _store.UpdateUserNotificationStateAsync(companyId, userNotificationId, state);
        }

        /// <summary>
        /// Updates the state of a user notification.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userNotificationId">The user notification identifier.</param>
        /// <param name="state">The new notification state.</param>
        public void UpdateUserNotificationState(int? companyId, Guid userNotificationId, UserNotificationState state)
        {
            _store.UpdateUserNotificationState(companyId, userNotificationId, state);
        }

        /// <summary>
        /// Updates the state of all user notifications for a user asynchronously.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">The new notification state.</param>
        public Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            return _store.UpdateAllUserNotificationStatesAsync(user, state);
        }

        /// <summary>
        /// Updates the state of all user notifications for a user.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">The new notification state.</param>
        public void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
            _store.UpdateAllUserNotificationStates(user, state);
        }

        /// <summary>
        /// Deletes a user notification asynchronously.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userNotificationId">The user notification identifier.</param>
        public Task DeleteUserNotificationAsync(int? companyId, Guid userNotificationId)
        {
            return _store.DeleteUserNotificationAsync(companyId, userNotificationId);
        }


        /// <summary>
        /// Deletes a user notification.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userNotificationId">The user notification identifier.</param>
        public void DeleteUserNotification(int? companyId, Guid userNotificationId)
        {
            _store.DeleteUserNotification(companyId, userNotificationId);
        }

        /// <summary>
        /// Deletes all user notifications for a user asynchronously.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">Optional notification state filter.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        public Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return _store.DeleteAllUserNotificationsAsync(user, state, startDate, endDate);
        }

        /// <summary>
        /// Deletes all user notifications for a user.
        /// </summary>
        /// <param name="user">The user identifier.</param>
        /// <param name="state">Optional notification state filter.</param>
        /// <param name="startDate">Optional start date filter.</param>
        /// <param name="endDate">Optional end date filter.</param>
        public void DeleteAllUserNotifications(UserIdentifier user, UserNotificationState? state = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            _store.DeleteAllUserNotifications(user, state, startDate, endDate);
        }
    }
}
