using System.Security.Claims;

namespace Kontecg.Runtime.Session
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
