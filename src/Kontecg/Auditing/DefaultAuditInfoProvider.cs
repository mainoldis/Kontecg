using Kontecg.Dependency;
using Kontecg.Extensions;

namespace Kontecg.Auditing
{
    /// <summary>
    ///     Default implementation of <see cref="IAuditInfoProvider" />.
    /// </summary>
    public class DefaultAuditInfoProvider : IAuditInfoProvider, ITransientDependency
    {
        public DefaultAuditInfoProvider()
        {
            ClientInfoProvider = NullClientInfoProvider.Instance;
        }

        public IClientInfoProvider ClientInfoProvider { get; set; }

        public virtual void Fill(AuditInfo auditInfo)
        {
            if (auditInfo.ClientIpAddress.IsNullOrEmpty())
            {
                auditInfo.ClientIpAddress = ClientInfoProvider.ClientIpAddress;
            }

            if (auditInfo.ClientInfo.IsNullOrEmpty())
            {
                auditInfo.ClientInfo = ClientInfoProvider.ClientInfo;
            }

            if (auditInfo.ClientName.IsNullOrEmpty())
            {
                auditInfo.ClientName = ClientInfoProvider.ComputerName;
            }
        }
    }
}
