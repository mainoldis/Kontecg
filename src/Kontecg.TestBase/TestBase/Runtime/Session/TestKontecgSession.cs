using System;
using Kontecg.Configuration.Startup;
using Kontecg.Dependency;
using Kontecg.MultiCompany;
using Kontecg.Runtime;
using Kontecg.Runtime.Session;

namespace Kontecg.TestBase.Runtime.Session
{
    public class TestKontecgSession : IKontecgSession, ISingletonDependency
    {
        private readonly ICompanyResolver _companyResolver;
        private readonly IMultiCompanyConfig _multiCompany;
        private readonly IAmbientScopeProvider<SessionOverride> _sessionOverrideScopeProvider;
        private int? _companyId;
        private long? _userId;

        public TestKontecgSession(
            IMultiCompanyConfig multiCompany,
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider,
            ICompanyResolver companyResolver)
        {
            _multiCompany = multiCompany;
            _sessionOverrideScopeProvider = sessionOverrideScopeProvider;
            _companyResolver = companyResolver;
        }

        public virtual long? UserId
        {
            get
            {
                if (_sessionOverrideScopeProvider.GetValue(KontecgSessionBase.SessionOverrideContextKey) != null)
                {
                    return _sessionOverrideScopeProvider.GetValue(KontecgSessionBase.SessionOverrideContextKey).UserId;
                }

                return _userId;
            }
            set => _userId = value;
        }

        public virtual int? CompanyId
        {
            get
            {
                if (!_multiCompany.IsEnabled)
                {
                    return 1;
                }

                if (_sessionOverrideScopeProvider.GetValue(KontecgSessionBase.SessionOverrideContextKey) != null)
                {
                    return _sessionOverrideScopeProvider.GetValue(KontecgSessionBase.SessionOverrideContextKey)
                        .CompanyId;
                }

                int? resolvedValue = _companyResolver.ResolveCompanyId();
                if (resolvedValue != null)
                {
                    return resolvedValue;
                }

                return _companyId;
            }
            set
            {
                if (!_multiCompany.IsEnabled && value != 1 && value != null)
                {
                    throw new KontecgException(
                        "Can not set CompanyId since multi-company is not enabled. Use IMultiCompanyConfig.IsEnabled to enable it.");
                }

                _companyId = value;
            }
        }

        public virtual MultiCompanySides MultiCompanySide => GetCurrentMultiCompanySide();

        public long? ImpersonatorUserId { get; set; }

        public int? ImpersonatorCompanyId { get; set; }

        public virtual IDisposable Use(int? companyId, long? userId)
        {
            return _sessionOverrideScopeProvider.BeginScope(KontecgSessionBase.SessionOverrideContextKey,
                new SessionOverride(companyId, userId));
        }

        protected virtual MultiCompanySides GetCurrentMultiCompanySide()
        {
            return _multiCompany.IsEnabled && !CompanyId.HasValue
                ? MultiCompanySides.Host
                : MultiCompanySides.Company;
        }
    }
}
