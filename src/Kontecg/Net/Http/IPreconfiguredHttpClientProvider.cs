using System.Net.Http;

namespace Kontecg.Net.Http
{
    public interface IPreconfiguredHttpClientProvider
    {
        HttpClient HttpClient { get; }
    }
}
