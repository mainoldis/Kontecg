using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Resolvers;
using JetBrains.Annotations;
using Kontecg.Dependency;
using Kontecg.EFCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private static readonly MethodInfo CreateOptionsMethod =
            typeof(DefaultDbContextResolver).GetMethod("CreateOptions", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        private readonly IIocResolver _iocResolver;

        public DefaultDbContextResolver(
            IIocResolver iocResolver,
            IDbContextTypeMatcher dbContextTypeMatcher)
        {
            _iocResolver = iocResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString, DbConnection existingConnection)
            where TDbContext : DbContext
        {
            Type dbContextType = typeof(TDbContext);
            Type concreteType = null;
            bool isAbstractDbContext = dbContextType.GetTypeInfo().IsAbstract;
            if (isAbstractDbContext)
            {
                concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);
            }

            try
            {
                if (isAbstractDbContext)
                {
                    return (TDbContext) _iocResolver.Resolve(concreteType, new
                    {
                        options = CreateOptionsForType(concreteType, connectionString, existingConnection)
                    });
                }

                return _iocResolver.Resolve<TDbContext>(new
                {
                    options = CreateOptions<TDbContext>(connectionString, existingConnection)
                });
            }
            catch (DependencyResolverException ex)
            {
                bool hasOptions = isAbstractDbContext ? HasOptions(concreteType) : HasOptions(dbContextType);
                if (!hasOptions)
                {
                    throw new AggregateException(
                        $"The parameter name of {dbContextType.Name}'s constructor must be 'options'", ex);
                }

                throw;
            }

            bool HasOptions(Type contextType)
            {
                return contextType.GetConstructors().Any(ctor =>
                {
                    ParameterInfo[] parameters = ctor.GetParameters();
                    return parameters.Length == 1 && parameters.FirstOrDefault()?.Name == "options";
                });
            }
        }

        protected virtual DbContextOptions<TDbContext> CreateOptions<TDbContext>([NotNull] string connectionString,
            [CanBeNull] DbConnection existingConnection) where TDbContext : DbContext
        {
            if (_iocResolver.IsRegistered<IKontecgDbContextConfigurer<TDbContext>>())
            {
                KontecgDbContextConfiguration<TDbContext> configuration =
                    new KontecgDbContextConfiguration<TDbContext>(connectionString, existingConnection);
                configuration.DbContextOptions.UseApplicationServiceProvider(_iocResolver.Resolve<IServiceProvider>());

                using IDisposableDependencyObjectWrapper<IKontecgDbContextConfigurer<TDbContext>> configurer =
                    _iocResolver.ResolveAsDisposable<IKontecgDbContextConfigurer<TDbContext>>();
                configurer.Object.Configure(configuration);

                return configuration.DbContextOptions.Options;
            }

            if (_iocResolver.IsRegistered<DbContextOptions<TDbContext>>())
            {
                return _iocResolver.Resolve<DbContextOptions<TDbContext>>();
            }

            throw new KontecgException(
                $"Could not resolve DbContextOptions for {typeof(TDbContext).AssemblyQualifiedName}.");
        }

        private object CreateOptionsForType(Type dbContextType, string connectionString,
            DbConnection existingConnection)
        {
            return CreateOptionsMethod.MakeGenericMethod(dbContextType)
                .Invoke(this, new object[] {connectionString, existingConnection});
        }
    }
}
