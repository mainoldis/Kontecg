namespace Kontecg.Net.Http
{
    public static class HttpSettingNames
    {
        /// <summary>
        ///     Kontecg.Net.Http.DefaultRequestHeader
        /// </summary>
        public const string XApplicationName = "Kontecg.Net.Http.DefaultRequestHeader";

        /// <summary>
        ///     Kontecg.Net.Http.UseDefaultProxy
        /// </summary>
        public const string UseDefaultProxy = "Kontecg.Net.Http.UseDefaultProxy";

        /// <summary>
        ///     Http related proxy settings.
        /// </summary>
        public static class Proxy
        {
            /// <summary>
            ///     Kontecg.Net.Http.Proxy.Uri
            /// </summary>
            public const string Uri = "Kontecg.Net.Http.Proxy.Uri";

            /// <summary>
            ///     Kontecg.Net.Http.Proxy.UserName
            /// </summary>
            public const string UserName = "Kontecg.Net.Http.Proxy.UserName";

            /// <summary>
            ///     Kontecg.Net.Http.Proxy.Password
            /// </summary>
            public const string Password = "Kontecg.Net.Http.Proxy.Password";

            /// <summary>
            ///     Kontecg.Net.Http.Proxy.Domain
            /// </summary>
            public const string Domain = "Kontecg.Net.Http.Proxy.Domain";

            /// <summary>
            ///     Kontecg.Net.Http.Proxy.BypassOnLocal
            /// </summary>
            public const string BypassOnLocal = "Kontecg.Net.Http.Proxy.BypassOnLocal";

            /// <summary>
            ///     Kontecg.Net.Http.Proxy.BypassList
            /// </summary>
            public const string BypassList = "Kontecg.Net.Http.Proxy.BypassList";
        }
    }
}
