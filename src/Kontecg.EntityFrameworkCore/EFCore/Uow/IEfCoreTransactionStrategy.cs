using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.EFCore.Uow
{
    public interface IEfCoreTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        Task<DbContext> CreateDbContextAsync<TDbContext>(
            string connectionString,
            IDbContextResolver dbContextResolver) where TDbContext : DbContext;

        void Commit();

        void Dispose(IIocResolver iocResolver);

        DbContext CreateDbContext<TDbContext>(
            string connectionString,
            IDbContextResolver dbContextResolver) where TDbContext : DbContext;
    }
}
