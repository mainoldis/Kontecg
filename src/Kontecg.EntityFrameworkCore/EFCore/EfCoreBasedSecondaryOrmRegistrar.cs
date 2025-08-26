using System;

namespace Kontecg.EFCore
{
    public class EfCoreBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
    {
        public EfCoreBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey { get; } = KontecgConsts.Orms.EntityFrameworkCore;
    }
}
