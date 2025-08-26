using System.Security.Claims;
using System.Threading;
using Kontecg.Dependency;

namespace Kontecg.Runtime.Session
{
    public class DefaultPrincipalAccessor : IPrincipalAccessor, ISingletonDependency
    {
        public static DefaultPrincipalAccessor Instance => new();
        public virtual ClaimsPrincipal Principal => Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}
