using System;
using System.Threading;
using Castle.Core.Logging;
using Kontecg.Runtime;

namespace Kontecg.Threading
{
    public abstract class CancellationTokenProviderBase : ICancellationTokenProvider
    {
        public const string CancellationTokenOverrideContextKey = "Kontecg.Threading.CancellationToken.Override";

        protected CancellationTokenProviderBase(
            IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
        {
            CancellationTokenOverrideScopeProvider = cancellationTokenOverrideScopeProvider;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        protected IAmbientScopeProvider<CancellationTokenOverride> CancellationTokenOverrideScopeProvider { get; }

        protected CancellationTokenOverride OverridedValue =>
            CancellationTokenOverrideScopeProvider.GetValue(CancellationTokenOverrideContextKey);

        public abstract CancellationToken Token { get; }

        public IDisposable Use(CancellationToken cancellationToken)
        {
            return CancellationTokenOverrideScopeProvider.BeginScope(CancellationTokenOverrideContextKey,
                new CancellationTokenOverride(cancellationToken));
        }
    }
}
