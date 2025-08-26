using System;

namespace Kontecg.EFCore
{
    public interface IDbContextTypeMatcher
    {
        void Populate(Type[] dbContextTypes);

        Type GetConcreteType(Type sourceDbContextType);
    }
}
