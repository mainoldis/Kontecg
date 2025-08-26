using Kontecg.Domain.Repositories;

namespace Kontecg.EFCore.Repositories
{
    public static class EfCoreAutoRepositoryTypes
    {
        static EfCoreAutoRepositoryTypes()
        {
            Default = new AutoRepositoryTypesAttribute(
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(EfCoreRepositoryBase<,>),
                typeof(EfCoreRepositoryBase<,,>)
            );
        }

        public static AutoRepositoryTypesAttribute Default { get; }
    }
}
