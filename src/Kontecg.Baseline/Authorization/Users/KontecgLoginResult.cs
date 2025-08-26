using System;
using System.Security.Claims;
using Kontecg.Localization;
using Kontecg.MultiCompany;

namespace Kontecg.Authorization.Users
{
    public class KontecgLoginResult<TCompany, TUser>
        where TCompany : KontecgCompany<TUser>
        where TUser : KontecgUserBase
    {
        public KontecgLoginResult(KontecgLoginResultType result, TCompany company = null, TUser user = null)
        {
            Result = result;
            Company = company;
            User = user;
        }

        public KontecgLoginResult(TCompany company, TUser user, ClaimsIdentity identity)
            : this(KontecgLoginResultType.Success, company)
        {
            User = user;
            Identity = identity;
        }

        public KontecgLoginResultType Result { get; }

        public ILocalizableString FailReason { get; private set; }

        public TCompany Company { get; }

        public TUser User { get; }

        public ClaimsIdentity Identity { get; }

        /// <summary>
        ///     This method can be used only when <see cref="Result" /> is
        ///     <see cref="KontecgLoginResultType.FailedForOtherReason" />.
        /// </summary>
        /// <param name="failReason">Localizable fail reason message</param>
        public void SetFailReason(ILocalizableString failReason)
        {
            if (Result != KontecgLoginResultType.FailedForOtherReason)
            {
                throw new Exception(
                    $"Can not set fail reason for result type {Result}, use this method only for KontecgLoginResultType.FailedForOtherReason result type!");
            }

            FailReason = failReason;
        }

        public string GetFailReason(ILocalizationContext localizationContext)
        {
            return FailReason == null ? string.Empty : FailReason?.Localize(localizationContext);
        }
    }
}
