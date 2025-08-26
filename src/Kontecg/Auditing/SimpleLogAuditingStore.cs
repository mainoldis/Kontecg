using System.Threading.Tasks;
using Castle.Core.Logging;

namespace Kontecg.Auditing
{
    /// <summary>
    ///     Implements <see cref="IAuditingStore" /> to simply write audits to logs.
    /// </summary>
    public class SimpleLogAuditingStore : IAuditingStore
    {
        public SimpleLogAuditingStore()
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static SimpleLogAuditingStore Instance { get; } = new();

        public ILogger Logger { get; set; }

        public Task SaveAsync(AuditInfo auditInfo)
        {
            if (auditInfo.Exception == null)
            {
                Logger.Info(auditInfo.ToString());
            }
            else
            {
                Logger.Warn(auditInfo.ToString());
            }

            return Task.FromResult(0);
        }

        public void Save(AuditInfo auditInfo)
        {
            if (auditInfo.Exception == null)
            {
                Logger.Info(auditInfo.ToString());
            }
            else
            {
                Logger.Warn(auditInfo.ToString());
            }
        }
    }
}
