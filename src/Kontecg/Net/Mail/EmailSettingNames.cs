namespace Kontecg.Net.Mail
{
    /// <summary>
    ///     Declares names of the settings defined by <see cref="EmailSettingProvider" />.
    /// </summary>
    public static class EmailSettingNames
    {
        /// <summary>
        ///     Kontecg.Net.Mail.DefaultFromAddress
        /// </summary>
        public const string DefaultFromAddress = "Kontecg.Net.Mail.DefaultFromAddress";

        /// <summary>
        ///     Kontecg.Net.Mail.DefaultFromDisplayName
        /// </summary>
        public const string DefaultFromDisplayName = "Kontecg.Net.Mail.DefaultFromDisplayName";

        /// <summary>
        ///     SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.Host
            /// </summary>
            public const string Host = "Kontecg.Net.Mail.Smtp.Host";

            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.Port
            /// </summary>
            public const string Port = "Kontecg.Net.Mail.Smtp.Port";

            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.UserName
            /// </summary>
            public const string UserName = "Kontecg.Net.Mail.Smtp.UserName";

            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.Password
            /// </summary>
            public const string Password = "Kontecg.Net.Mail.Smtp.Password";

            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.Domain
            /// </summary>
            public const string Domain = "Kontecg.Net.Mail.Smtp.Domain";

            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.EnableSsl
            /// </summary>
            public const string EnableSsl = "Kontecg.Net.Mail.Smtp.EnableSsl";

            /// <summary>
            ///     Kontecg.Net.Mail.Smtp.UseDefaultCredentials
            /// </summary>
            public const string UseDefaultCredentials = "Kontecg.Net.Mail.Smtp.UseDefaultCredentials";
        }
    }
}
