namespace Kontecg.Net.Http
{
    public interface IHttpClientConfiguration
    {
        /// <summary>
        ///     Header.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        ///     Use system proxy?
        /// </summary>
        bool UseDefaultProxy { get; }

        /// <summary>
        ///     Proxy address on form http[s]://ip:port
        /// </summary>
        string ProxyAddress { get; }

        /// <summary>
        ///     User name to login to proxy server.
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///     Password to login to proxy server.
        /// </summary>
        string Password { get; }

        /// <summary>
        ///     Domain name to login to proxy server.
        /// </summary>
        string Domain { get; }

        /// <summary>
        ///     Bypass on local address?
        /// </summary>
        bool BypassOnLocal { get; }

        /// <summary>
        ///     Domain name to login to proxy server.
        /// </summary>
        string[] BypassList { get; }
    }
}
