using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.EFCore.Configuration;
using Kontecg.EFCore.Repositories;
using Kontecg.EFCore.Uow;
using Kontecg.Modules;
using Kontecg.Orm;
using Kontecg.Reflection;
using Kontecg.Reflection.Extensions;

namespace Kontecg.EFCore
{
    /// <summary>
    ///     This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    [DependsOn(typeof(KontecgKernelModule))]
    public class KontecgEntityFrameworkCoreModule : KontecgModule
    {
        private readonly ITypeFinder _typeFinder;

        public KontecgEntityFrameworkCoreModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IKontecgEfCoreConfiguration, KontecgEfCoreConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KontecgEntityFrameworkCoreModule).GetAssembly());

            IocManager.IocContainer.Register(
                Component.For(typeof(IDbContextProvider<>))
                    .ImplementedBy(typeof(UnitOfWorkDbContextProvider<>))
                    .LifestyleTransient()
            );

            RegisterGenericRepositoriesAndMatchDbContexes();
        }

        private void RegisterGenericRepositoriesAndMatchDbContexes()
        {
            Type[] dbContextTypes =
                _typeFinder.Find(type =>
                {
                    TypeInfo typeInfo = type.GetTypeInfo();
                    return typeInfo.IsPublic &&
                           !typeInfo.IsAbstract &&
                           typeInfo.IsClass &&
                           typeof(KontecgDbContext).IsAssignableFrom(type);
                });

            if (dbContextTypes.IsNullOrEmpty())
            {
                Logger.Warn("No class found derived from KontecgDbContext.");
                return;
            }

            using IScopedIocResolver scope = IocManager.CreateScope();
            foreach (Type dbContextType in dbContextTypes)
            {
                Logger.Debug("Registering DbContext: " + dbContextType.AssemblyQualifiedName);

                scope.Resolve<IEfGenericRepositoryRegistrar>().RegisterForDbContext(dbContextType, IocManager,
                    EfCoreAutoRepositoryTypes.Default);

                IocManager.IocContainer.Register(
                    Component.For<ISecondaryOrmRegistrar>()
                        .Named(Guid.NewGuid().ToString("N"))
                        .Instance(new EfCoreBasedSecondaryOrmRegistrar(dbContextType,
                            scope.Resolve<IDbContextEntityFinder>()))
                        .LifestyleTransient()
                );
            }

            scope.Resolve<IDbContextTypeMatcher>().Populate(dbContextTypes);
        }
    }
}
