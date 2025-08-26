﻿using Kontecg.Configuration;
using Kontecg.Extensions;

namespace Kontecg.Net.Mail
{
    /// <summary>
    ///     Implementation of <see cref="IEmailSenderConfiguration" /> that reads settings
    ///     from <see cref="ISettingManager" />.
    /// </summary>
    public abstract class EmailSenderConfiguration : IEmailSenderConfiguration
    {
        protected readonly ISettingManager SettingManager;

        /// <summary>
        ///     Creates a new <see cref="EmailSenderConfiguration" />.
        /// </summary>
        protected EmailSenderConfiguration(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }

        public virtual string DefaultFromAddress => GetNotEmptySettingValue(EmailSettingNames.DefaultFromAddress);

        public virtual string DefaultFromDisplayName =>
            SettingManager.GetSettingValue(EmailSettingNames.DefaultFromDisplayName);

        /// <summary>
        ///     Gets a setting value by checking. Throws <see cref="KontecgException" /> if it's null or empty.
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <returns>Value of the setting</returns>
        protected string GetNotEmptySettingValue(string name)
        {
            string value = SettingManager.GetSettingValue(name);

            if (value.IsNullOrEmpty())
            {
                throw new KontecgException($"Setting value for '{name}' is null or empty!");
            }

            return value;
        }
    }
}
