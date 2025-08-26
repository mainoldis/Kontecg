using System;
using Kontecg.Application.Services.Dto;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Timing;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Represents a published notification for a company/user.
    /// </summary>
    [Serializable]
    public class CompanyNotification : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyNotification" /> class.
        /// </summary>
        public CompanyNotification()
        {
            CreationTime = Clock.Now;
        }

        /// <summary>
        ///     Company Id.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     Unique notification name.
        /// </summary>
        public string NotificationName { get; set; }

        /// <summary>
        ///     Notification data.
        /// </summary>
        public NotificationData Data { get; set; }

        /// <summary>
        ///     Gets or sets the type of the entity.
        /// </summary>
        [Obsolete(
            "(De)serialization of System.Type is bad and not supported. See https://github.com/dotnet/corefx/issues/42712")]
        public Type EntityType { get; set; }

        /// <summary>
        ///     Name of the entity type (including namespaces).
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        ///     Entity id.
        /// </summary>
        public object EntityId { get; set; }

        /// <summary>
        ///     Severity.
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
