using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Kontecg.Collections.Extensions;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Kontecg.EFCore.Extensions;
using Kontecg.Transactions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kontecg.EFCore.Uow
{
    public class DbContextEfCoreTransactionStrategy : IEfCoreTransactionStrategy, ITransientDependency
    {
        public DbContextEfCoreTransactionStrategy()
        {
            ActiveTransactions = new Dictionary<string, ActiveTransactionInfo>(StringComparer.OrdinalIgnoreCase);
        }

        protected UnitOfWorkOptions Options { get; private set; }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactions { get; }

        public void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;
        }

        public async Task<DbContext> CreateDbContextAsync<TDbContext>(string connectionString,
            IDbContextResolver dbContextResolver) where TDbContext : DbContext
        {
            DbContext dbContext;

            ActiveTransactionInfo activeTransaction = ActiveTransactions.GetOrDefault(connectionString);
            if (activeTransaction == null)
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(connectionString, null);

                IDbContextTransaction dbTransaction = await dbContext.Database.BeginTransactionAsync(
                    (Options.IsolationLevel ?? IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel());

                activeTransaction = new ActiveTransactionInfo(dbTransaction, dbContext);
                ActiveTransactions[connectionString] = activeTransaction;
            }
            else
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(
                    connectionString,
                    activeTransaction.DbContextTransaction.GetDbTransaction().Connection
                );

                if (dbContext.HasRelationalTransactionManager())
                {
                    await dbContext.Database.UseTransactionAsync(
                        activeTransaction.DbContextTransaction.GetDbTransaction());
                }
                else
                {
                    await dbContext.Database.BeginTransactionAsync();
                }

                activeTransaction.AttendedDbContexts.Add(dbContext);
            }

            return dbContext;
        }

        public void Commit()
        {
            foreach (ActiveTransactionInfo activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Commit();

                foreach (DbContext dbContext in activeTransaction.AttendedDbContexts)
                {
                    if (dbContext.HasRelationalTransactionManager())
                    {
                        continue; //Relational databases use the shared transaction
                    }

                    dbContext.Database.CommitTransaction();
                }
            }
        }

        public void Dispose(IIocResolver iocResolver)
        {
            foreach (ActiveTransactionInfo activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbContextTransaction.Dispose();

                foreach (DbContext attendedDbContext in activeTransaction.AttendedDbContexts)
                {
                    iocResolver.Release(attendedDbContext);
                }

                iocResolver.Release(activeTransaction.StarterDbContext);
            }

            ActiveTransactions.Clear();
        }

        public DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext
        {
            DbContext dbContext;

            ActiveTransactionInfo activeTransaction = ActiveTransactions.GetOrDefault(connectionString);
            if (activeTransaction == null)
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(connectionString, null);

                IDbContextTransaction dbTransaction = dbContext.Database.BeginTransaction(
                    (Options.IsolationLevel ?? IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel());

                activeTransaction = new ActiveTransactionInfo(dbTransaction, dbContext);
                ActiveTransactions[connectionString] = activeTransaction;
            }
            else
            {
                dbContext = dbContextResolver.Resolve<TDbContext>(
                    connectionString,
                    activeTransaction.DbContextTransaction.GetDbTransaction().Connection
                );

                if (dbContext.HasRelationalTransactionManager())
                {
                    dbContext.Database.UseTransaction(activeTransaction.DbContextTransaction.GetDbTransaction());
                }
                else
                {
                    dbContext.Database.BeginTransaction();
                }

                activeTransaction.AttendedDbContexts.Add(dbContext);
            }

            return dbContext;
        }
    }
}
