using System;
using Kontecg.Authorization.Users;

namespace Kontecg.Runtime.Session
{
    public static class KontecgSessionExtensions
    {
        public static bool IsUser(this IKontecgSession session, KontecgUserBase user)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return session.CompanyId == user.CompanyId &&
                   session.UserId.HasValue &&
                   session.UserId.Value == user.Id;
        }
    }
}
