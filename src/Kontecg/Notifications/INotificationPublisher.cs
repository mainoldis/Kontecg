using System;
using System.Threading.Tasks;
using Kontecg.Domain.Entities;
using Kontecg.Runtime.Session;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Used to publish notifications.
    /// </summary>
    public interface INotificationPublisher
    {
        /// <summary>
        ///     Publishes a new notification.
        /// </summary>
        /// <param name="notificationName">Unique notification name</param>
        /// <param name="data">Notification data (optional)</param>
        /// <param name="entityIdentifier">The entity identifier if this notification is related to an entity</param>
        /// <param name="severity">Notification severity</param>
        /// <param name="userIds">
        ///     Target user id(s).
        ///     Used to send notification to specific user(s) (without checking the subscription).
        ///     If this is null/empty, the notification is sent to subscribed users.
        /// </param>
        /// <param name="excludedUserIds">
        ///     Excluded user id(s).
        ///     This can be set to exclude some users while publishing notifications to subscribed users.
        ///     It's normally not set if <paramref name="userIds" /> is set.
        /// </param>
        /// <param name="companyIds">
        ///     Target company id(s).
        ///     Used to send notification to subscribed users of specific company(s).
        ///     This should not be set if <paramref name="userIds" /> is set.
        ///     <see cref="NotificationPublisher.AllCompanies" /> can be passed to indicate all companies.
        ///     But this can only work in a single database approach (all companies are stored in host database).
        ///     If this is null, then it's automatically set to the current company on <see cref="IKontecgSession.CompanyId" />.
        /// </param>
        /// <param name="targetNotifiers">
        ///     Which realtime notifiers should handle this notification. Given notifier must be added to
        ///     the INotificationConfiguration.Notifiers
        /// </param>
        Task PublishAsync(
            string notificationName,
            NotificationData data = null,
            EntityIdentifier entityIdentifier = null,
            NotificationSeverity severity = NotificationSeverity.Info,
            UserIdentifier[] userIds = null,
            UserIdentifier[] excludedUserIds = null,
            int?[] companyIds = null,
            Type[] targetNotifiers = null);
    }
}
