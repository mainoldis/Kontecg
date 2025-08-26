using JetBrains.Annotations;

namespace Kontecg.RealTime
{
    public static class OnlineClientExtensions
    {
        [CanBeNull]
        public static UserIdentifier ToUserIdentifierOrNull(this IOnlineClient onlineClient)
        {
            return onlineClient.UserId.HasValue
                ? new UserIdentifier(onlineClient.CompanyId, onlineClient.UserId.Value)
                : null;
        }
    }
}
