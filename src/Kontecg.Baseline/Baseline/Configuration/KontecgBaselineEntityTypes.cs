using System;
using Kontecg.Authorization.Roles;
using Kontecg.Authorization.Users;
using Kontecg.MultiCompany;

namespace Kontecg.Baseline.Configuration
{
    public class KontecgBaselineEntityTypes : IKontecgBaselineEntityTypes
    {
        private Type _company;
        private Type _role;
        private Type _user;

        public Type User
        {
            get => _user;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(KontecgUserBase).IsAssignableFrom(value))
                {
                    throw new KontecgException(value.AssemblyQualifiedName + " should be derived from " +
                                               typeof(KontecgUserBase).AssemblyQualifiedName);
                }

                _user = value;
            }
        }

        public Type Role
        {
            get => _role;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(KontecgRoleBase).IsAssignableFrom(value))
                {
                    throw new KontecgException(value.AssemblyQualifiedName + " should be derived from " +
                                               typeof(KontecgRoleBase).AssemblyQualifiedName);
                }

                _role = value;
            }
        }

        public Type Company
        {
            get => _company;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(KontecgCompanyBase).IsAssignableFrom(value))
                {
                    throw new KontecgException(value.AssemblyQualifiedName + " should be derived from " +
                                               typeof(KontecgCompanyBase).AssemblyQualifiedName);
                }

                _company = value;
            }
        }
    }
}
