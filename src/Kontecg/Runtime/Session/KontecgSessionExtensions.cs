namespace Kontecg.Runtime.Session
{
    /// <summary>
    ///     Extension methods for <see cref="IKontecgSession" />.
    /// </summary>
    public static class KontecgSessionExtensions
    {
        /// <summary>
        ///     Gets current User's Id.
        ///     Throws <see cref="KontecgException" /> if <see cref="IKontecgSession.UserId" /> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current User's Id.</returns>
        public static long GetUserId(this IKontecgSession session)
        {
            if (!session.UserId.HasValue)
            {
                throw new KontecgException("Session.UserId is null! Probably, user is not logged in.");
            }

            return session.UserId.Value;
        }

        /// <summary>
        ///     Gets current Company's Id.
        ///     Throws <see cref="KontecgException" /> if <see cref="IKontecgSession.CompanyId" /> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current Company's Id.</returns>
        /// <exception cref="KontecgException"></exception>
        public static int GetCompanyId(this IKontecgSession session)
        {
            if (!session.CompanyId.HasValue)
            {
                throw new KontecgException(
                    "Session.CompanyId is null! Possible problems: No user logged in or current logged in user in a host user (CompanyId is always null for host users).");
            }

            return session.CompanyId.Value;
        }

        /// <summary>
        ///     Creates <see cref="UserIdentifier" /> from given session.
        ///     Returns null if <see cref="IKontecgSession.UserId" /> is null.
        /// </summary>
        /// <param name="session">The session.</param>
        public static UserIdentifier ToUserIdentifier(this IKontecgSession session)
        {
            return session.UserId == null
                ? null
                : new UserIdentifier(session.CompanyId, session.GetUserId());
        }
    }
}
