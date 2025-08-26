using System;
using System.Collections.Generic;
using Kontecg.Collections.Extensions;
using Kontecg.Json;

namespace Kontecg.Notifications
{
    /// <summary>
    ///     Used to store data for a notification.
    ///     It can be directly used or can be derived.
    /// </summary>
    [Serializable]
    public class NotificationData
    {
        private readonly Dictionary<string, object> _properties;

        /// <summary>
        ///     Createa a new NotificationData object.
        /// </summary>
        public NotificationData()
        {
            _properties = new Dictionary<string, object>();
        }

        /// <summary>
        ///     Gets notification data type name.
        ///     It returns the full class name by default.
        /// </summary>
        public virtual string Type => GetType().FullName;

        /// <summary>
        ///     Shortcut to set/get <see cref="Properties" />.
        /// </summary>
        public object this[string key]
        {
            get => Properties.GetOrDefault(key);
            set => Properties[key] = value;
        }

        /// <summary>
        ///     Can be used to add custom properties to this notification.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get => _properties;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                /* Not assign value, but add dictionary items. This is required for backward compability. */
                foreach (KeyValuePair<string, object> keyValue in value)
                {
                    if (!_properties.ContainsKey(keyValue.Key))
                    {
                        _properties[keyValue.Key] = keyValue.Value;
                    }
                }
            }
        }

        public override string ToString()
        {
            return this.ToJsonString();
        }
    }
}
