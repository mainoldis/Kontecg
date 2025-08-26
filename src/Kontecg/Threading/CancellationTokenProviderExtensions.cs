using System.Threading;

namespace Kontecg.Threading
{
    public static class CancellationTokenProviderExtensions
    {
        public static CancellationToken FallbackToProvider(this ICancellationTokenProvider provider, CancellationToken preferedValue = default)
        {
            return preferedValue == default || preferedValue == CancellationToken.None
                ? provider.Token
                : preferedValue;
        }
    }
}
