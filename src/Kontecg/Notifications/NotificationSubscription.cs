using System;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Timing;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Represents a user subscription to a notification.
    /// </summary>
    public class NotificationSubscription : IHasCreationTime
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationSubscription" /> class.
        /// </summary>
        public NotificationSubscription()
        {
            CreationTime = Clock.Now;
        }

        /// <summary>
        ///     Company id of the subscribed user.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     User Id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///     Notification unique name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        ///     Entity type.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        ///     Name of the entity type (including namespaces).
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        ///     Entity Id.
        /// </summary>
        public object EntityId { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
