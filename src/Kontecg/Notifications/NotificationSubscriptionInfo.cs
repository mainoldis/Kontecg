using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Json;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Used to store a notification subscription.
    /// </summary>
    [Table("notification_subscriptions", Schema = "log")]
    public class NotificationSubscriptionInfo : CreationAuditedEntity<Guid>, IMayHaveCompany
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationSubscriptionInfo" /> class.
        /// </summary>
        public NotificationSubscriptionInfo()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationSubscriptionInfo" /> class.
        /// </summary>
        public NotificationSubscriptionInfo(Guid id, int? companyId, long userId, string notificationName,
            EntityIdentifier entityIdentifier = null)
        {
            Id = id;
            CompanyId = companyId;
            NotificationName = notificationName;
            UserId = userId;
            EntityTypeName = entityIdentifier?.Type.FullName;
            EntityTypeAssemblyQualifiedName =
                entityIdentifier?.Type.AssemblyQualifiedName;
            EntityId = entityIdentifier?.Id.ToJsonString();
        }

        /// <summary>
        ///     User Id.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        ///     Notification unique name.
        /// </summary>
        [StringLength(NotificationInfo.MaxNotificationNameLength)]
        public virtual string NotificationName { get; set; }

        /// <summary>
        ///     Gets/sets entity type name, if this is an entity level notification.
        ///     It's FullName of the entity type.
        /// </summary>
        [StringLength(NotificationInfo.MaxEntityTypeNameLength)]
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        ///     AssemblyQualifiedName of the entity type.
        /// </summary>
        [StringLength(NotificationInfo.MaxEntityTypeAssemblyQualifiedNameLength)]
        public virtual string EntityTypeAssemblyQualifiedName { get; set; }

        /// <summary>
        ///     Gets/sets primary key of the entity, if this is an entity level notification.
        /// </summary>
        [StringLength(NotificationInfo.MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        /// <summary>
        ///     Company id of the subscribed user.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
