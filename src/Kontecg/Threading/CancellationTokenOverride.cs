using System.Threading;

namespace Kontecg.Threading
{
    public class CancellationTokenOverride
    {
        public CancellationTokenOverride(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; }
    }
}
