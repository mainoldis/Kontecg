using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Timing;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Used to store a user notification.
    /// </summary>
    [Serializable]
    [Table("user_notifications", Schema = "log")]
    public class UserNotificationInfo : Entity<Guid>, IHasCreationTime, IMayHaveCompany
    {
        public UserNotificationInfo()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserNotificationInfo" /> class.
        /// </summary>
        /// <param name="id"></param>
        public UserNotificationInfo(Guid id)
        {
            Id = id;
            State = UserNotificationState.Unread;
            CreationTime = Clock.Now;
        }

        /// <summary>
        ///     User Id.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        ///     Notification Id.
        /// </summary>
        [Required]
        public virtual Guid CompanyNotificationId { get; set; }

        /// <summary>
        ///     Current state of the user notification.
        /// </summary>
        public virtual UserNotificationState State { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the user notification.
        /// </summary>
        /// <remarks>
        /// This property is set automatically when the notification is created.
        /// </remarks>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        ///     Company Id.
        /// </summary>
        public virtual int? CompanyId { get; set; }
    }
}
