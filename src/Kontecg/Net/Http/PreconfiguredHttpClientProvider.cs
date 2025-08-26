using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using Kontecg.Extensions;
using Velopack;

namespace Kontecg.Net.Http
{
    public class PreconfiguredHttpClientProvider : IPreconfiguredHttpClientProvider
    {
        private readonly IHttpClientConfiguration _configuration;

        public PreconfiguredHttpClientProvider(IHttpClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ProductInfoHeaderValue UserAgent =>
            new("KONTECG", VelopackRuntimeInfo.VelopackProductVersion.Version.ToString());

        private HttpClient BuildClient()
        {
            HttpClientHandler handler = new();
            // enable TLS support
            // TLS 1.0 and 1.1 are enabled for backward compatibility and should be disabled in the future
            // for security reasons
            handler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            // disable SSLv3 support for security reasons
#pragma warning disable CS0618
            handler.SslProtocols &= ~SslProtocols.Ssl3;
#pragma warning restore CS0618

            handler.AllowAutoRedirect = true;
            handler.MaxAutomaticRedirections = 10;
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (!_configuration.UseDefaultProxy)
            {
                WebProxy proxy = new(_configuration.ProxyAddress, _configuration.BypassOnLocal,
                    _configuration.BypassList);
                string userName = _configuration.UserName;
                if (!userName.IsNullOrEmpty())
                {
                    proxy.UseDefaultCredentials = false;
                    string password = _configuration.Password;
                    string domain = _configuration.Domain;
                    proxy.Credentials = !domain.IsNullOrEmpty()
                        ? new NetworkCredential(userName, password, domain)
                        : new NetworkCredential(userName, password);
                }

                handler.Proxy = proxy;
            }

            HttpClient httpClient = new(handler, true);
            httpClient.DefaultRequestHeaders.UserAgent.Add(UserAgent);
            return httpClient;
        }

        public HttpClient HttpClient => BuildClient();
    }
}
