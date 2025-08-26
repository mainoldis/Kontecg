using System;
using System.Transactions;
using Castle.Core.Logging;
using Kontecg.Data;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.EFCore;
using Kontecg.Extensions;
using Kontecg.MultiCompany;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Baseline.EFCore
{
    public abstract class KontecgDbMigrator<TDbContext> : IKontecgDbMigrator, ITransientDependency
        where TDbContext : DbContext
    {
        private readonly IConnectionStringResolver _connectionStringResolver;
        protected readonly IDbContextResolver DbContextResolver;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        protected KontecgDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver)
        {
            UnitOfWorkManager = unitOfWorkManager;
            _connectionStringResolver = connectionStringResolver;
            DbContextResolver = dbContextResolver;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public virtual void CreateOrMigrateForHost()
        {
            CreateOrMigrateForHost(null);
        }

        public virtual void CreateOrMigrateForCompany(KontecgCompanyBase company)
        {
            CreateOrMigrateForCompany(company, null);
        }

        public virtual void CreateOrMigrateForHost(Action<TDbContext, ILogger> seedAction)
        {
            CreateOrMigrate(null, seedAction);
        }

        public virtual void CreateOrMigrateForCompany(KontecgCompanyBase company, Action<TDbContext, ILogger> seedAction)
        {
            if (company.ConnectionString.IsNullOrEmpty())
            {
                return;
            }

            CreateOrMigrate(company, seedAction);
        }

        protected virtual void CreateOrMigrate(KontecgCompanyBase company, Action<TDbContext, ILogger> seedAction)
        {
            DbPerCompanyConnectionStringResolveArgs args = new DbPerCompanyConnectionStringResolveArgs(
                company?.Id,
                company == null ? MultiCompanySides.Host : MultiCompanySides.Company
            ) {["DbContextType"] = typeof(TDbContext), ["DbContextConcreteType"] = typeof(TDbContext)};

            string nameOrConnectionString = ConnectionStringHelper.GetConnectionString(
                _connectionStringResolver.GetNameOrConnectionString(args)
            );

            using IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using TDbContext dbContext = DbContextResolver.Resolve<TDbContext>(nameOrConnectionString, null);
            Logger.Info("Creating database");
            dbContext.Database.Migrate();
            Logger.Info("Database created");
            Logger.Info("Seeding with initial data");
            seedAction?.Invoke(dbContext, Logger);
            Logger.Info("Seed was saved");
            UnitOfWorkManager.Current.SaveChanges();
            uow.Complete();
        }
    }
}
