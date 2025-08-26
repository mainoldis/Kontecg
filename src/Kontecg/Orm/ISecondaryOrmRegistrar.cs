using Kontecg.Dependency;
using Kontecg.Domain.Repositories;

namespace Kontecg.Orm
{
    public interface ISecondaryOrmRegistrar
    {
        string OrmContextKey { get; }

        void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes);
    }
}
