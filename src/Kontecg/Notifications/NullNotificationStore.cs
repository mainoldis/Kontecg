using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kontecg.Notifications
{
    /// <summary>
    /// Null pattern implementation of <see cref="INotificationStore" />.
    /// Provides a safe, no-operation implementation for notification storage.
    /// </summary>
    /// <remarks>
    /// This class is used to avoid null checks and provides default behavior for notification storage operations.
    /// </remarks>
    public class NullNotificationStore : INotificationStore
    {
        /// <inheritdoc />
        public Task InsertSubscriptionAsync(NotificationSubscriptionInfo subscription)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task DeleteSubscriptionAsync(UserIdentifier user, string notificationName, string entityTypeName,
            string entityId)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task InsertNotificationAsync(NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task<NotificationInfo> GetNotificationOrNullAsync(Guid notificationId)
        {
            return Task.FromResult(null as NotificationInfo);
        }

        /// <inheritdoc />
        public Task InsertUserNotificationAsync(UserNotificationInfo userNotification)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName,
            string entityTypeName = null, string entityId = null)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        /// <inheritdoc />
        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(int?[] companyIds,
            string notificationName,
            string entityTypeName, string entityId)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        /// <inheritdoc />
        public Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(UserIdentifier user)
        {
            return Task.FromResult(new List<NotificationSubscriptionInfo>());
        }

        /// <inheritdoc />
        public Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, string entityTypeName,
            string entityId)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task UpdateUserNotificationStateAsync(int? notificationId, Guid userNotificationId,
            UserNotificationState state)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task DeleteUserNotificationAsync(int? notificationId, Guid userNotificationId)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task DeleteAllUserNotificationsAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task<List<UserNotificationInfoWithNotificationInfo>> GetUserNotificationsWithNotificationsAsync(
            UserIdentifier user, UserNotificationState? state = null, int skipCount = 0,
            int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
        {
            return Task.FromResult(new List<UserNotificationInfoWithNotificationInfo>());
        }

        /// <inheritdoc />
        public Task<int> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task<UserNotificationInfoWithNotificationInfo> GetUserNotificationWithNotificationOrNullAsync(
            int? companyId, Guid userNotificationId)
        {
            return Task.FromResult((UserNotificationInfoWithNotificationInfo) null);
        }

        /// <inheritdoc />
        public Task InsertCompanyNotificationAsync(CompanyNotificationInfo companyNotificationInfo)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task DeleteNotificationAsync(NotificationInfo notification)
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public void InsertSubscription(NotificationSubscriptionInfo subscription)
        {
        }

        /// <inheritdoc />
        public void DeleteSubscription(UserIdentifier user, string notificationName, string entityTypeName,
            string entityId)
        {
        }

        /// <inheritdoc />
        public void InsertNotification(NotificationInfo notification)
        {
        }

        /// <inheritdoc />
        public NotificationInfo GetNotificationOrNull(Guid notificationId)
        {
            return new NotificationInfo();
        }

        /// <inheritdoc />
        public void InsertUserNotification(UserNotificationInfo userNotification)
        {
        }

        /// <inheritdoc />
        public List<NotificationSubscriptionInfo> GetSubscriptions(string notificationName, string entityTypeName,
            string entityId)
        {
            return new List<NotificationSubscriptionInfo>();
        }

        /// <inheritdoc />
        public List<NotificationSubscriptionInfo> GetSubscriptions(int?[] companyIds, string notificationName,
            string entityTypeName, string entityId)
        {
            return new List<NotificationSubscriptionInfo>();
        }

        /// <inheritdoc />
        public List<NotificationSubscriptionInfo> GetSubscriptions(UserIdentifier user)
        {
            return new List<NotificationSubscriptionInfo>();
        }

        /// <inheritdoc />
        public bool IsSubscribed(UserIdentifier user, string notificationName, string entityTypeName, string entityId)
        {
            return false;
        }

        /// <inheritdoc />
        public void UpdateUserNotificationState(int? companyId, Guid userNotificationId, UserNotificationState state)
        {
        }

        /// <inheritdoc />
        public void UpdateAllUserNotificationStates(UserIdentifier user, UserNotificationState state)
        {
        }

        /// <inheritdoc />
        public void DeleteUserNotification(int? companyId, Guid userNotificationId)
        {
        }

        /// <inheritdoc />
        public void DeleteAllUserNotifications(UserIdentifier user,
            UserNotificationState? state = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
        }

        /// <inheritdoc />
        public List<UserNotificationInfoWithNotificationInfo> GetUserNotificationsWithNotifications(UserIdentifier user,
            UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return new List<UserNotificationInfoWithNotificationInfo>();
        }

        /// <inheritdoc />
        public int GetUserNotificationCount(UserIdentifier user, UserNotificationState? state = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            return 0;
        }

        /// <inheritdoc />
        public UserNotificationInfoWithNotificationInfo GetUserNotificationWithNotificationOrNull(int? companyId,
            Guid userNotificationId)
        {
            return null;
        }

        /// <inheritdoc />
        public void InsertCompanyNotification(CompanyNotificationInfo companyNotificationInfo)
        {
        }

        /// <inheritdoc />
        public void DeleteNotification(NotificationInfo notification)
        {
        }

        /// <inheritdoc />
        public Task<List<GetNotificationsCreatedByUserOutput>> GetNotificationsPublishedByUserAsync(UserIdentifier user,
            string notificationName, DateTime? startDate,
            DateTime? endDate)
        {
            return Task.FromResult(new List<GetNotificationsCreatedByUserOutput>());
        }
    }
}
