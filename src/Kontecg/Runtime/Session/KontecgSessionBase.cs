using System;
using Kontecg.Configuration.Startup;
using Kontecg.MultiCompany;

namespace Kontecg.Runtime.Session
{
    public abstract class KontecgSessionBase : IKontecgSession
    {
        public const string SessionOverrideContextKey = "Kontecg.Runtime.Session.Override";

        protected KontecgSessionBase(IMultiCompanyConfig multiCompany,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider)
        {
            MultiCompany = multiCompany;
            SessionOverrideScopeProvider = sessionOverrideScopeProvider;
        }

        public IMultiCompanyConfig MultiCompany { get; }

        protected SessionOverride OverridedValue => SessionOverrideScopeProvider.GetValue(SessionOverrideContextKey);
        protected IAmbientScopeProvider<SessionOverride> SessionOverrideScopeProvider { get; }

        public abstract long? UserId { get; }

        public abstract int? CompanyId { get; }

        public virtual MultiCompanySides MultiCompanySide =>
            MultiCompany.IsEnabled && !CompanyId.HasValue
                ? MultiCompanySides.Host
                : MultiCompanySides.Company;

        public abstract long? ImpersonatorUserId { get; }

        public abstract int? ImpersonatorCompanyId { get; }

        public IDisposable Use(int? companyId, long? userId)
        {
            return SessionOverrideScopeProvider.BeginScope(SessionOverrideContextKey,
                new SessionOverride(companyId, userId));
        }
    }
}
