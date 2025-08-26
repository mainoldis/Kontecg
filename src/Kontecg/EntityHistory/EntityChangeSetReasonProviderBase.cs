using System;
using Castle.Core.Logging;
using Kontecg.Runtime;

namespace Kontecg.EntityHistory
{
    public abstract class EntityChangeSetReasonProviderBase : IEntityChangeSetReasonProvider
    {
        public const string ReasonOverrideContextKey = "Kontecg.EntityHistory.Reason.Override";

        protected EntityChangeSetReasonProviderBase(IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider)
        {
            ReasonOverrideScopeProvider = reasonOverrideScopeProvider;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        protected ReasonOverride OverridedValue => ReasonOverrideScopeProvider.GetValue(ReasonOverrideContextKey);
        protected IAmbientScopeProvider<ReasonOverride> ReasonOverrideScopeProvider { get; }

        public abstract string Reason { get; }

        public IDisposable Use(string reason)
        {
            return ReasonOverrideScopeProvider.BeginScope(ReasonOverrideContextKey, new ReasonOverride(reason));
        }
    }
}
