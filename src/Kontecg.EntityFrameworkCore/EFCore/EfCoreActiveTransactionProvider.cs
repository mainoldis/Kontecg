using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Kontecg.Data;
using Kontecg.Dependency;
using Kontecg.MultiCompany;
using Kontecg.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kontecg.EFCore
{
    public class EfCoreActiveTransactionProvider : IActiveTransactionProvider, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public EfCoreActiveTransactionProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task<IDbTransaction> GetActiveTransactionAsync(ActiveTransactionProviderArgs args)
        {
            DbContext context = await GetDbContextAsync(args);
            return context.Database.CurrentTransaction?.GetDbTransaction();
        }

        public IDbTransaction GetActiveTransaction(ActiveTransactionProviderArgs args)
        {
            DbContext context = GetDbContext(args);
            return context.Database.CurrentTransaction?.GetDbTransaction();
        }

        public async Task<IDbConnection> GetActiveConnectionAsync(ActiveTransactionProviderArgs args)
        {
            DbContext context = await GetDbContextAsync(args);
            return context.Database.GetDbConnection();
        }

        public IDbConnection GetActiveConnection(ActiveTransactionProviderArgs args)
        {
            DbContext context = GetDbContext(args);
            return context.Database.GetDbConnection();
        }

        private async Task<DbContext> GetDbContextAsync(ActiveTransactionProviderArgs args)
        {
            Type dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType((Type) args["ContextType"]);

            using IDisposableDependencyObjectWrapper dbContextProviderWrapper =
                _iocResolver.ResolveAsDisposable(dbContextProviderType);
            MethodInfo method = dbContextProviderWrapper.Object.GetType()
                .GetMethod(
                    nameof(IDbContextProvider<KontecgDbContext>.GetDbContextAsync),
                    new[] {typeof(MultiCompanySides)}
                );

            object result = await ReflectionHelper.InvokeAsync(method, dbContextProviderWrapper.Object,
                (MultiCompanySides?) args["MultiCompanySide"]);

            return result as DbContext;
        }

        private DbContext GetDbContext(ActiveTransactionProviderArgs args)
        {
            Type dbContextProviderType = typeof(IDbContextProvider<>).MakeGenericType((Type) args["ContextType"]);

            using IDisposableDependencyObjectWrapper dbContextProviderWrapper =
                _iocResolver.ResolveAsDisposable(dbContextProviderType);
            MethodInfo method = dbContextProviderWrapper.Object.GetType()
                .GetMethod(
                    nameof(IDbContextProvider<KontecgDbContext>.GetDbContext),
                    new[] {typeof(MultiCompanySides)}
                );

            return (DbContext) method.Invoke(
                dbContextProviderWrapper.Object,
                new object[] {(MultiCompanySides?) args["MultiCompanySide"]}
            );
        }
    }
}
