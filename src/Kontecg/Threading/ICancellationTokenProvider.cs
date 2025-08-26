using System;
using System.Threading;

namespace Kontecg.Threading
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
        IDisposable Use(CancellationToken cancellationToken);
    }
}
