using System;

namespace Kontecg.Configuration
{
    /// <summary>
    ///     Represents a setting information.
    /// </summary>
    [Serializable]
    public class SettingInfo
    {
        /// <summary>
        ///     Creates a new <see cref="SettingInfo" /> object.
        /// </summary>
        public SettingInfo()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="SettingInfo" /> object.
        /// </summary>
        /// <param name="companyId">CompanyId for this setting. CompanyId is null if this setting is not Company level.</param>
        /// <param name="userId">UserId for this setting. UserId is null if this setting is not user level.</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public SettingInfo(int? companyId, long? userId, string name, string value)
        {
            CompanyId = companyId;
            UserId = userId;
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     CompanyId for this setting.
        ///     CompanyId is null if this setting is not Company level.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        ///     UserId for this setting.
        ///     UserId is null if this setting is not user level.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        ///     Unique name of the setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value of the setting.
        /// </summary>
        public string Value { get; set; }
    }
}
