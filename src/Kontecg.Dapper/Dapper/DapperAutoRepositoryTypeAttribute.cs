using System;
using JetBrains.Annotations;
using Kontecg.Domain.Repositories;

namespace Kontecg.Dapper
{
    public class DapperAutoRepositoryTypeAttribute : AutoRepositoryTypesAttribute
    {
        public DapperAutoRepositoryTypeAttribute(
            [NotNull] Type repositoryInterface,
            [NotNull] Type repositoryInterfaceWithPrimaryKey,
            [NotNull] Type repositoryImplementation,
            [NotNull] Type repositoryImplementationWithPrimaryKey)
            : base(repositoryInterface, repositoryInterfaceWithPrimaryKey, repositoryImplementation,
                repositoryImplementationWithPrimaryKey)
        {
        }
    }
}
