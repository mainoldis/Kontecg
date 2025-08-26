using Kontecg.Configuration;
using Kontecg.Dependency;
using Kontecg.Extensions;

namespace Kontecg.Net.Http
{
    public class HttpClientConfiguration : IHttpClientConfiguration, ITransientDependency
    {
        private readonly ISettingManager _settingManager;

        public HttpClientConfiguration(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        ///     Header.
        /// </summary>
        public virtual string ApplicationName => GetNotEmptySettingValue(HttpSettingNames.XApplicationName);

        /// <summary>
        ///     Use system proxy?
        /// </summary>
        public virtual bool UseDefaultProxy => _settingManager.GetSettingValue<bool>(HttpSettingNames.UseDefaultProxy);

        /// <summary>
        ///     Proxy address on form http[s]://ip:port
        /// </summary>
        public virtual string ProxyAddress => GetNotEmptySettingValue(HttpSettingNames.Proxy.Uri);

        /// <summary>
        ///     User name to login to proxy server.
        /// </summary>
        public virtual string UserName => GetNotEmptySettingValue(HttpSettingNames.Proxy.UserName);

        /// <summary>
        ///     Password to login to proxy server.
        /// </summary>
        public virtual string Password => GetNotEmptySettingValue(HttpSettingNames.Proxy.Password);

        /// <summary>
        ///     Domain name to login to proxy server.
        /// </summary>
        public virtual string Domain => _settingManager.GetSettingValue(HttpSettingNames.Proxy.Domain);

        /// <summary>
        ///     Bypass on local address?
        /// </summary>
        public virtual bool BypassOnLocal =>
            _settingManager.GetSettingValue<bool>(HttpSettingNames.Proxy.BypassOnLocal);

        /// <summary>
        ///     Domain name to login to proxy server.
        /// </summary>
        public virtual string[] BypassList => GetNotEmptySettingValue(HttpSettingNames.Proxy.BypassList).Split(";");

        /// <summary>
        ///     Gets a setting value by checking. Throws <see cref="KontecgException" /> if it's null or empty.
        /// </summary>
        /// <param name="name">Name of the setting</param>
        /// <returns>Value of the setting</returns>
        private string GetNotEmptySettingValue(string name)
        {
            string value = _settingManager.GetSettingValue(name);

            if (value.IsNullOrEmpty())
            {
                throw new KontecgException($"Setting value for '{name}' is null or empty!");
            }

            return value;
        }
    }
}
