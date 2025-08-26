using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Runtime.Session;
using Kontecg.Threading;

namespace Kontecg.Workflows
{
    public class WorkflowCompanyAccessor : ITransientDependency
    {
        private readonly ICancellationTokenProvider _cancellationTokenProvider;

        public WorkflowCompanyAccessor(ICancellationTokenProvider cancellationTokenProvider)
        {
            _cancellationTokenProvider = cancellationTokenProvider;
            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        public virtual Task<string> GetCompanyIdAsync()
        {
            return Task.FromResult(KontecgSession.CompanyId?.ToString());
        }
    }
}
