using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;

namespace Kontecg.EFCore
{
    public abstract class DbContextTypeMatcher<TBaseDbContext> : IDbContextTypeMatcher, ISingletonDependency
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly Dictionary<Type, List<Type>> _dbContextTypes;

        protected DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _dbContextTypes = new Dictionary<Type, List<Type>>();
        }

        public void Populate(Type[] dbContextTypes)
        {
            foreach (Type dbContextType in dbContextTypes)
            {
                List<Type> types = new List<Type>();

                AddWithBaseTypes(dbContextType, types);

                foreach (Type type in types)
                {
                    Add(type, dbContextType);
                }
            }
        }

        //TODO: GetConcreteType method can be optimized by extracting/caching MultiCompanySideAttribute attributes for DbContexes.

        public virtual Type GetConcreteType(Type sourceDbContextType)
        {
            //TODO: This can also get MultiCompanySide to filter dbcontexes

            if (!sourceDbContextType.GetTypeInfo().IsAbstract)
            {
                return sourceDbContextType;
            }

            //Get possible concrete types for given DbContext type
            List<Type> allTargetTypes = _dbContextTypes.GetOrDefault(sourceDbContextType);

            if (allTargetTypes.IsNullOrEmpty())
            {
                throw new KontecgException("Could not find a concrete implementation of given DbContext type: " +
                                           sourceDbContextType.AssemblyQualifiedName);
            }

            if (allTargetTypes.Count == 1)
                //Only one type does exists, return it
            {
                return allTargetTypes[0];
            }

            CheckCurrentUow();

            MultiCompanySides currentCompanySide = GetCurrentCompanySide();

            List<Type> multiCompanySideContexes = GetMultiCompanySideContextTypes(allTargetTypes, currentCompanySide);

            return multiCompanySideContexes.Count == 1
                ? multiCompanySideContexes[0]
                : GetDefaultDbContextType(
                    multiCompanySideContexes.Count > 1 ? multiCompanySideContexes : allTargetTypes, sourceDbContextType,
                    currentCompanySide);
        }

        private void CheckCurrentUow()
        {
            if (_currentUnitOfWorkProvider.Current == null)
            {
                throw new KontecgException("GetConcreteType method should be called in a UOW.");
            }
        }

        private MultiCompanySides GetCurrentCompanySide()
        {
            return _currentUnitOfWorkProvider.Current.GetCompanyId() == null
                ? MultiCompanySides.Host
                : MultiCompanySides.Company;
        }

        private static List<Type> GetMultiCompanySideContextTypes(List<Type> dbContextTypes,
            MultiCompanySides companySide)
        {
            return dbContextTypes.Where(type =>
            {
                object[] attrs = type.GetTypeInfo().GetCustomAttributes(typeof(MultiCompanySideAttribute), true)
                    .ToArray();
                if (attrs.IsNullOrEmpty())
                {
                    return false;
                }

                return ((MultiCompanySideAttribute) attrs[0]).Side.HasFlag(companySide);
            }).ToList();
        }

        private static Type GetDefaultDbContextType(List<Type> dbContextTypes, Type sourceDbContextType,
            MultiCompanySides companySide)
        {
            List<Type> filteredTypes = dbContextTypes
                .Where(type => !type.GetTypeInfo().IsDefined(typeof(AutoRepositoryTypesAttribute), true))
                .ToList();

            if (filteredTypes.Count == 1)
            {
                return filteredTypes[0];
            }

            filteredTypes = filteredTypes
                .Where(type => type.GetTypeInfo().IsDefined(typeof(DefaultDbContextAttribute), true))
                .ToList();

            if (filteredTypes.Count == 1)
            {
                return filteredTypes[0];
            }

            throw new KontecgException(
                $"Found more than one concrete type for given DbContext Type ({sourceDbContextType}) define MultiCompanySideAttribute with {companySide}. Found types: {dbContextTypes.Select(c => c.AssemblyQualifiedName).JoinAsString(", ")}.");
        }

        private static void AddWithBaseTypes(Type dbContextType, List<Type> types)
        {
            types.Add(dbContextType);
            if (dbContextType != typeof(TBaseDbContext))
            {
                AddWithBaseTypes(dbContextType.GetTypeInfo().BaseType, types);
            }
        }

        private void Add(Type sourceDbContextType, Type targetDbContextType)
        {
            if (!_dbContextTypes.ContainsKey(sourceDbContextType))
            {
                _dbContextTypes[sourceDbContextType] = new List<Type>();
            }

            _dbContextTypes[sourceDbContextType].Add(targetDbContextType);
        }
    }

    public class DbContextTypeMatcher : DbContextTypeMatcher<KontecgDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}
