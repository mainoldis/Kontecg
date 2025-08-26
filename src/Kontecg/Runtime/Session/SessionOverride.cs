namespace Kontecg.Runtime.Session
{
    public class SessionOverride
    {
        public SessionOverride(int? companyId, long? userId)
        {
            CompanyId = companyId;
            UserId = userId;
        }

        public long? UserId { get; }

        public int? CompanyId { get; }
    }
}
