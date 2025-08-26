using Kontecg.Configuration.Startup;
using Kontecg.MultiCompany;
using Kontecg.Runtime.Remoting;

namespace Kontecg.Runtime.Session
{
    /// <summary>
    ///     Implements null object pattern for <see cref="IKontecgSession" />.
    /// </summary>
    public class NullKontecgSession : KontecgSessionBase
    {
        private NullKontecgSession()
            : base(
                new MultiCompanyConfig(),
                new DataContextAmbientScopeProvider<SessionOverride>(new AsyncLocalAmbientDataContext())
            )
        {
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullKontecgSession Instance { get; } = new();

        /// <inheritdoc />
        public override long? UserId => null;

        /// <inheritdoc />
        public override int? CompanyId => null;

        /// <inheritdoc />
        public override MultiCompanySides MultiCompanySide => MultiCompanySides.Company;

        /// <inheritdoc />
        public override long? ImpersonatorUserId => null;

        /// <inheritdoc />
        public override int? ImpersonatorCompanyId => null;
    }
}
