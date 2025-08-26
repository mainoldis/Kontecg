using System;
using Kontecg.Localization;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class LocalizableMessageNotificationData : NotificationData
    {
        private LocalizableString _message;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalizableMessageNotificationData" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LocalizableMessageNotificationData(LocalizableString message)
        {
            Message = message;
        }

        /// <summary>
        ///     Needed for serialization.
        /// </summary>
        private LocalizableMessageNotificationData()
        {
        }

        /// <summary>
        ///     The message.
        /// </summary>
        public LocalizableString Message
        {
            get => _message ?? this[nameof(Message)] as LocalizableString;
            set
            {
                this[nameof(Message)] = value;
                _message = value;
            }
        }
    }
}
