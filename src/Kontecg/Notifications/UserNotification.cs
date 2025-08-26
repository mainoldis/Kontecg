using System;
using Kontecg.Application.Services.Dto;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Represents a notification sent to a user.
    /// </summary>
    [Serializable]
    public class UserNotification : EntityDto<Guid>, IUserIdentifier
    {
        /// <summary>
        ///     Current state of the user notification.
        /// </summary>
        public UserNotificationState State { get; set; }

        /// <summary>
        ///     The notification.
        /// </summary>
        public CompanyNotification Notification { get; set; }

        /// <summary>
        ///     CompanyId.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     User Id.
        /// </summary>
        public long UserId { get; set; }
    }
}
