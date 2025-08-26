using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Uow
{
    /// <summary>
    ///     Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfCoreUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IDbContextResolver _dbContextResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;
        private readonly IEfCoreTransactionStrategy _transactionStrategy;

        /// <summary>
        ///     Creates a new <see cref="EfCoreUnitOfWork" />.
        /// </summary>
        public EfCoreUnitOfWork(
            IIocResolver iocResolver,
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkFilterExecuter filterExecuter,
            IDbContextResolver dbContextResolver,
            IUnitOfWorkDefaultOptions defaultOptions,
            IDbContextTypeMatcher dbContextTypeMatcher,
            IEfCoreTransactionStrategy transactionStrategy)
            : base(
                connectionStringResolver,
                defaultOptions,
                filterExecuter)
        {
            IocResolver = iocResolver;
            _dbContextResolver = dbContextResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
            _transactionStrategy = transactionStrategy;

            ActiveDbContexts = new Dictionary<string, DbContext>(StringComparer.OrdinalIgnoreCase);
        }

        protected IDictionary<string, DbContext> ActiveDbContexts { get; }

        protected IIocResolver IocResolver { get; }

        public override void SaveChanges()
        {
            foreach (DbContext dbContext in GetAllActiveDbContexts())
            {
                SaveChangesInDbContext(dbContext);
            }
        }

        public override async Task SaveChangesAsync()
        {
            foreach (DbContext dbContext in GetAllActiveDbContexts())
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }

        public IReadOnlyList<DbContext> GetAllActiveDbContexts()
        {
            return ActiveDbContexts.Values.ToImmutableList();
        }

        public virtual async Task<TDbContext> GetOrCreateDbContextAsync<TDbContext>(
            MultiCompanySides? multiCompanySide = null, string name = null)
            where TDbContext : DbContext
        {
            Type concreteDbContextType = _dbContextTypeMatcher.GetConcreteType(typeof(TDbContext));

            ConnectionStringResolveArgs connectionStringResolveArgs = new ConnectionStringResolveArgs(multiCompanySide)
            {
                ["DbContextType"] = typeof(TDbContext),
                ["DbContextConcreteType"] = concreteDbContextType
            };

            string connectionString = await ResolveConnectionStringAsync(connectionStringResolveArgs);

            string dbContextKey = concreteDbContextType.FullName + "#" + connectionString;
            if (name != null)
            {
                dbContextKey += "#" + name;
            }

            if (ActiveDbContexts.TryGetValue(dbContextKey, out DbContext dbContext))
            {
                return (TDbContext) dbContext;
            }

            if (Options.IsTransactional == true)
            {
                dbContext = await _transactionStrategy
                    .CreateDbContextAsync<TDbContext>(connectionString, _dbContextResolver);
            }
            else
            {
                dbContext = _dbContextResolver.Resolve<TDbContext>(connectionString, null);
            }

            if (dbContext is IShouldInitializeDbContext kontecgDbContext)
            {
                kontecgDbContext.Initialize(new KontecgEfDbContextInitializationContext(this));
            }

            ActiveDbContexts[dbContextKey] = dbContext;

            return (TDbContext) dbContext;
        }

        public virtual TDbContext GetOrCreateDbContext<TDbContext>(
            MultiCompanySides? multiCompanySide = null, string name = null)
            where TDbContext : DbContext
        {
            Type concreteDbContextType = _dbContextTypeMatcher.GetConcreteType(typeof(TDbContext));

            ConnectionStringResolveArgs connectionStringResolveArgs = new ConnectionStringResolveArgs(multiCompanySide)
            {
                ["DbContextType"] = typeof(TDbContext),
                ["DbContextConcreteType"] = concreteDbContextType
            };

            string connectionString = ResolveConnectionString(connectionStringResolveArgs);

            string dbContextKey = concreteDbContextType.FullName + "#" + connectionString;
            if (name != null)
            {
                dbContextKey += "#" + name;
            }

            if (ActiveDbContexts.TryGetValue(dbContextKey, out DbContext dbContext))
            {
                return (TDbContext) dbContext;
            }

            if (Options.IsTransactional == true)
            {
                dbContext = _transactionStrategy
                    .CreateDbContext<TDbContext>(connectionString, _dbContextResolver);
            }
            else
            {
                dbContext = _dbContextResolver.Resolve<TDbContext>(connectionString, null);
            }

            if (dbContext is IShouldInitializeDbContext kontecgDbContext)
            {
                kontecgDbContext.Initialize(new KontecgEfDbContextInitializationContext(this));
            }

            ActiveDbContexts[dbContextKey] = dbContext;

            return (TDbContext) dbContext;
        }

        protected override void BeginUow()
        {
            if (Options.IsTransactional == true)
            {
                _transactionStrategy.InitOptions(Options);
            }
        }

        protected override void CompleteUow()
        {
            SaveChanges();
            CommitTransaction();
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            CommitTransaction();
        }

        protected override void DisposeUow()
        {
            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Dispose(IocResolver);
            }
            else
            {
                foreach (DbContext context in GetAllActiveDbContexts())
                {
                    Release(context);
                }
            }

            ActiveDbContexts.Clear();
        }

        protected virtual void SaveChangesInDbContext(DbContext dbContext)
        {
            dbContext.SaveChanges();
        }

        protected virtual Task SaveChangesInDbContextAsync(DbContext dbContext)
        {
            return dbContext.SaveChangesAsync();
        }

        protected virtual void Release(DbContext dbContext)
        {
            dbContext.Dispose();
            IocResolver.Release(dbContext);
        }

        private void CommitTransaction()
        {
            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Commit();
            }
        }

        //private static void ObjectContext_ObjectMaterialized(DbContext dbContext, ObjectMaterializedEventArgs e)
        //{
        //    var entityType = ObjectContext.GetObjectType(e.Entity.GetType());

        //    dbContext.Configuration.AutoDetectChangesEnabled = false;
        //    var previousState = dbContext.Entry(e.Entity).State;

        //    DateTimePropertyInfoHelper.NormalizeDatePropertyKinds(e.Entity, entityType);

        //    dbContext.Entry(e.Entity).State = previousState;
        //    dbContext.Configuration.AutoDetectChangesEnabled = true;
        //}
    }
}
