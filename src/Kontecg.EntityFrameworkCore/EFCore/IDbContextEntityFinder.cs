using System;
using System.Collections.Generic;
using Kontecg.Domain.Entities;

namespace Kontecg.EFCore
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}
