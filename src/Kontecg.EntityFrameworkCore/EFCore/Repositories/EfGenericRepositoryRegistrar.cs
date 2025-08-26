using System;
using System.Reflection;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Kontecg.Dependency;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Repositories;
using Kontecg.Reflection.Extensions;

namespace Kontecg.EFCore.Repositories
{
    public class EfGenericRepositoryRegistrar : IEfGenericRepositoryRegistrar, ITransientDependency
    {
        private readonly IDbContextEntityFinder _dbContextEntityFinder;

        public EfGenericRepositoryRegistrar(IDbContextEntityFinder dbContextEntityFinder)
        {
            _dbContextEntityFinder = dbContextEntityFinder;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void RegisterForDbContext(
            Type dbContextType,
            IIocManager iocManager,
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute)
        {
            AutoRepositoryTypesAttribute autoRepositoryAttr =
                dbContextType.GetTypeInfo().GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>() ??
                defaultAutoRepositoryTypesAttribute;

            RegisterForDbContext(
                dbContextType,
                iocManager,
                autoRepositoryAttr.RepositoryInterface,
                autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey,
                autoRepositoryAttr.RepositoryImplementation,
                autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
            );

            if (autoRepositoryAttr.WithDefaultRepositoryInterfaces)
            {
                RegisterForDbContext(
                    dbContextType,
                    iocManager,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterface,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterfaceWithPrimaryKey,
                    autoRepositoryAttr.RepositoryImplementation,
                    autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
                );
            }
        }

        private void RegisterForDbContext(
            Type dbContextType,
            IIocManager iocManager,
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            foreach (EntityTypeInfo entityTypeInfo in _dbContextEntityFinder.GetEntityTypeInfos(dbContextType))
            {
                Type primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    Type genericRepositoryType = repositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        Type implType = repositoryImplementation.GetGenericArguments().Length == 1
                            ? repositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                            : repositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType,
                                entityTypeInfo.EntityType);

                        iocManager.IocContainer.Register(
                            Component
                                .For(genericRepositoryType)
                                .ImplementedBy(implType)
                                .Named(Guid.NewGuid().ToString("N"))
                                .LifestyleTransient()
                        );
                    }
                }

                Type genericRepositoryTypeWithPrimaryKey =
                    repositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    Type implType = repositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                        ? repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType,
                            primaryKeyType)
                        : repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType,
                            entityTypeInfo.EntityType, primaryKeyType);

                    iocManager.IocContainer.Register(
                        Component
                            .For(genericRepositoryTypeWithPrimaryKey)
                            .ImplementedBy(implType)
                            .Named(Guid.NewGuid().ToString("N"))
                            .LifestyleTransient()
                    );
                }
            }
        }
    }
}
