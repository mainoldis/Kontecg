using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     A notification distributed to it's related company.
    /// </summary>
    [Table("company_notifications", Schema = "log")]
    public class CompanyNotificationInfo : CreationAuditedEntity<Guid>, IMayHaveCompany
    {
        public CompanyNotificationInfo()
        {
        }

        public CompanyNotificationInfo(Guid id, int? companyId, NotificationInfo notification)
        {
            Id = id;
            CompanyId = companyId;
            NotificationName = notification.NotificationName;
            Data = notification.Data;
            DataTypeName = notification.DataTypeName;
            EntityTypeName = notification.EntityTypeName;
            EntityTypeAssemblyQualifiedName = notification.EntityTypeAssemblyQualifiedName;
            EntityId = notification.EntityId;
            Severity = notification.Severity;
        }

        /// <summary>
        ///     Unique notification name.
        /// </summary>
        [Required]
        [StringLength(NotificationInfo.MaxNotificationNameLength)]
        public virtual string NotificationName { get; set; }

        /// <summary>
        ///     Notification data as JSON string.
        /// </summary>
        [StringLength(NotificationInfo.MaxDataLength)]
        public virtual string Data { get; set; }

        /// <summary>
        ///     Type of the JSON serialized <see cref="Data" />.
        ///     It's AssemblyQualifiedName of the type.
        /// </summary>
        [StringLength(NotificationInfo.MaxDataTypeNameLength)]
        public virtual string DataTypeName { get; set; }

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
        ///     Notification severity.
        /// </summary>
        public virtual NotificationSeverity Severity { get; set; }

        /// <summary>
        ///     Company id of the subscribed user.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
