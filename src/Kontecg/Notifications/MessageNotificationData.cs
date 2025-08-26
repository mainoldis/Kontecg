using System;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Can be used to store a simple message as notification data.
    /// </summary>
    [Serializable]
    public class MessageNotificationData : NotificationData
    {
        private string _message;

        public MessageNotificationData(string message)
        {
            Message = message;
        }

        /// <summary>
        ///     Needed for serialization.
        /// </summary>
        private MessageNotificationData()
        {
        }

        /// <summary>
        ///     The message.
        /// </summary>
        public string Message
        {
            get => _message ?? this[nameof(Message)] as string;
            set
            {
                this[nameof(Message)] = value;
                _message = value;
            }
        }
    }
}
