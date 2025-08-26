using System.Threading;
using Kontecg.Runtime.Remoting;

namespace Kontecg.Threading
{
    public class NullCancellationTokenProvider : CancellationTokenProviderBase
    {
        private NullCancellationTokenProvider()
            : base(
                new DataContextAmbientScopeProvider<CancellationTokenOverride>(new AsyncLocalAmbientDataContext()))
        {
        }

        public static NullCancellationTokenProvider Instance { get; } = new();

        public override CancellationToken Token => CancellationToken.None;
    }
}
