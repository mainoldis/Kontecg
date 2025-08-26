using System;
using Microsoft.AspNetCore.Identity;

namespace Kontecg.IdentityFramework
{
    public class KontecgIdentityBuilder : IdentityBuilder
    {
        public KontecgIdentityBuilder(IdentityBuilder identityBuilder, Type roleType, Type companyType)
            : base(identityBuilder.UserType, roleType, identityBuilder.Services)
        {
            CompanyType = companyType;
        }

        public Type CompanyType { get; }
    }
}
