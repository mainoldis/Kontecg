using System;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;

namespace Kontecg.EFCore.Repositories
{
    public interface IEfGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager,
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute);
    }
}
