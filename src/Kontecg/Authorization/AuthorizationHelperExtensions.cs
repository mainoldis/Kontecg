using System.Threading.Tasks;

namespace Kontecg.Authorization
{
    public static class AuthorizationHelperExtensions
    {
        public static async Task AuthorizeAsync(this IAuthorizationHelper authorizationHelper,
            IKontecgAuthorizeAttribute authorizeAttribute)
        {
            await authorizationHelper.AuthorizeAsync(new[] {authorizeAttribute});
        }

        public static void Authorize(this IAuthorizationHelper authorizationHelper,
            IKontecgAuthorizeAttribute authorizeAttribute)
        {
            authorizationHelper.Authorize(new[] {authorizeAttribute});
        }
    }
}
